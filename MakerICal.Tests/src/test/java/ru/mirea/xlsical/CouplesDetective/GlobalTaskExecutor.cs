/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using ru.mirea.xlsical.interpreter;
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;
using ru.mirea.xlsical.Server;

namespace ru.mirea.xlsical.CouplesDetective
{
    public class GlobalTaskExecutor
    {
        private static readonly string[] excelFiles201812 =
            new string[]
            {
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_563794600_6766608827827586509_FTI_Stromynka-1-kurs-1-sem-.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_564794200_-3578232590848298760_FTI_Stromynka-3-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_564794200_1285739816391932333_FTI_Stromynka-2-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_567793700_7599244429924950312_FTI_Stromynka-4-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_568793700_-189588281424640915_FTI-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_568793700_1519655368465447043_FTI-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_568793700_8234805916613096711_FTI_Stromynka-5-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_570796_-7091446714654852372_3054.FTI-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_570796_5864375867025986705_FTI-5k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_570796_5994248421785909380_FTI-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_571795100_-6794537177437047055_IINTEGU-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_571795100_-7933938956058297015_Zach_FTI-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_571795100_6131436554301044635_Zach_FTI-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_572795100_-4079916737417052549_IINTEGU-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_572795100_-5461732317332499261_IINTEGU-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_572795100_-6281406551599403533_INTEGU_3k_zaochn_-18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_572795100_5237191234236243376_IINTEGU-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_572795100_6684499524842844833_INTEGU_2k_zaochn_-18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_573793300_-6497274183981032951_Zach_IINTEGU-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_573793300_-846064367633904619_INTEGU_4k_zaochn_-18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_573793300_3247913048774953940_Zach_IINTEGU-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_573793300_8171388690345456451_Zach_IINTEGU-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_574794200_-8155066669884463036_IIT-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_574794200_3017082845926387332_Zach_IINTEGU-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_574794200_3380843044010916712_IIT-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_574794200_4099119350624465244_IIT-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_574794200_7029344313366176665_IIT-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_579794600_2577104522937712177_Zach_IIT-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_-7771087226855090801_Zach_IIT-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_-8216746045091105454_Zach_IIT-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_-8247887796289677588_3102.Raspisanie.IK-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_-9007124484197187946_IK-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_1501317253419310408_Zach_IIT-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_1804105010647177166_IK-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_580794600_4334665205860753813_IK-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_-4758662330662635902_Zach_IK-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_-7179513701457464662_Zach_IK-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_1695337742390964998_Zach_IK-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_2503933714149128456_Zach_IK-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_4100678164486412769_Zach_IK-5k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_581794100_647372913887039851_IK-5k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_582793700_-1970195133180601537_KBiSP-5-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_582793700_-5306190605533831635_KBiSP-4-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_582793700_-7974644522066630518_KBiSP-3-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_582793700_5555302942095636904_KBiSP-1-kurs-1-sem-.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_582793700_673874914767316925_KBiSP-2-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_-3073895456962058752_IRTS-5k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_-5398009264452439715_IRTS-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_4137056421752117980_IRTS-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_4799711613558828382_IRTS-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_5577004508165705697_3096.Raspisanie.IRTS-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_584793200_7690592348725729128_Zach_IRTS-1k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_585795_5496414563832722694_Zach_IRTS-2k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_586795_3295376064283909203_Zach_IRTS-3k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_597795_-4289816454614730493_Zach_IRTS-5k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_597795_2250253424629334653_Zach_IRTS-4k-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_598793700_4570680801792655035_itht_bak_2k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_598793700_476800263689526115_itht_bak_1k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_601794600_-1349870999506867568_itht_bak_3k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_601794600_-807103726064467729_itht_bak_4k_FOZO_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_601794600_8950563242114111442_itht_bak_3k_FOZO_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_601794600_899256376087878173_itht_bak_4k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_602794600_-561027847927335591_itht_bak_5k_FOZO_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_602794600_4968821860428871713_IEP-1-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_-2428604681148177150_IEP_3-kurs_vechernee.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_-5818916808009479684_IEP_2-kurs_zaochnoe.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_-7699974774362267819_IEP-2-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_2304696668005136658_IEP_4-kurs_vechernee.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_7453869238055629277_IEP-4-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_603794600_8275736078924281358_IEP-3-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_-411272585230209657_3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_-4203759638956468769_4-kurs-RTU-MIREA.XLSX",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_-6119853156855980332_3-kurs-RTU-MIREA.XLSX",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_1313768209752293252_IEP_3-kurs_zaochnoe.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_6441599110156555188_IEP_4-kurs_zaochnoe.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_7983017922265659033_5-kurs-RTU-MIREA.XLSX",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_8467302665054749858_4-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_604793600_8623527276628669616_2-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_612794100_-178022734891835186_5-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_612794100_-5598663660499220619_3-kurs-zaochniki.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_612794100_-8758670777405668939_4-kurs-zaochniki.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_612794100_1402877848318632564_3-kurs-vecherniki.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_612794100_482090541695919551_4-kurs-vecherniki.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_-3008033448528782501_IINTEGU-1k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_-369155845634205606_IINTEGU-2k_mag_zaoch_-18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_-5433364374605603327_2895.FTI_Stromynka-1-kurs-1-sem-MAGI.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_-5941562957343772856_FTI-2k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_3259623393568143929_IINTEGU-2k_mag_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_3473103573057174578_FTI_Stromynka-2-kurs-1-sem.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_613793600_9088127290834999883_FTI-1k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_-1137053027688175805_IKBSP-1-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_-1952124080277246813_IRTS-1k_mag_-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_-4872478307384090465_IKib_2k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_-7166144729538290163_IIT-2k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_4226567113724852833_IIT-1k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_5695331321665367636_IKib_1k-mag-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_614793600_7586946225100760482_IKBSP-2-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_-1552348637185570217_IRTS-2k_mag_-18_19-osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_-2032500298317406355_IEP_1-kurs_Magi_och.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_-5798269774519948921_IEP_2-kurs_Magi_och.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_-7348216035955819696_IEP_2-kurs_Magi_zaoch.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_-883136222797597169_3059.Raspisanie.itht_mag_1k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_615793200_6404993693985057325_itht_mag_2k_18_19_osen.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_622792200_3564068414017022146_2-kurs-MAGISTRY.XLSX",
                // need fast "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_622792200_8852235371680950173_2-kurs-MAGISTRY.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_638847900_-2276311418277754768_Institut-IT-2-kurs.xlsx",
                // need fast "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_638847900_-4217560698878680202_Institut-INTEGU-2-kurs.xlsx",
                // need fast "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_639847_-1663218377095180992_Institut-Kibernetika.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_639847_-312703427610516416_Institut-TKHT-2-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_639847_-5223520564697399301_Institut-KBSP-2-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_639847_-5793954046251822290_Institut-RTS-2-kurs.xlsx",
                // need fast "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_639847_-6000164318512669580_Institut-FTI-2-kurs.xlsx",
                // need fast "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_640847_-5039153050870218744_Institut-EiP-2-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_640847_3181413907927372660_Institut-IT-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_640847_6208163444387675646_Institut-INTEGU-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_641847900_-657632595437275826_Institut-KBSP-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_641847900_-8097893338571921460_Institut-Kibernetika-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_642847500_-8237296325511636816_Institut-FTI-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_642847500_4951022374759159802_Institut-RTS-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_642847500_8539195783563768101_Institut-TKHT-3-kurs.xlsx",
                "MakerICal.Tests.tests.big.excel.2018-12-10T16-08-36_645846500_-5165008637843634938_2755.Raspisanie.01.09.2018-Raspisanie.xls"
            };

        private static readonly IEnumerable<Stream> excelFiles201812Streams = new EnumerableResourceStreams(excelFiles201812);

        private static readonly PercentReady PR_init = new PercentReady(
                subscribers: new SampleConsoleTransferPercentReady("GlobalTaskExecutor.cs: ").TransferValue
        );

        public static readonly ExternalDataUpdater externalDataUpdater =
                new ExternalDataUpdater(excelFiles201812Streams, new List<Teacher>());

        public static readonly CoupleHistorian coupleHistorian;

        public static readonly FileInfo fileForCacheCoupleHistorian = new FileInfo("tests/big/GlobalTaskExecutor.java.dat");

        static GlobalTaskExecutor()
        {
            CoupleHistorian coupleHistorian1;
            fileForCacheCoupleHistorian.Directory.Create();
            coupleHistorian1 = new CoupleHistorian(
                DateTimeZoneProviders.Tzdb["Europe/Moscow"].AtStartOfDay(new LocalDate(2018, 9, 1)),
                externalDataUpdater,
                new DetectiveDate(),
                PR_init,
                fileForCacheCoupleHistorian);
            coupleHistorian = coupleHistorian1;
        }

        public static readonly TaskExecutor taskExecutor =
                new TaskExecutor(coupleHistorian);
    }
}
