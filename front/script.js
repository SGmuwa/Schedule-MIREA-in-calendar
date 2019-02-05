$('document').ready(function(){

	var flagForSettings = false;
	var flagForTimezones = false;
	var flagForDiscription = false;
	var zonesArr = [

	  "(GMT+14:00) Pacific/Apia",
	  "(GMT+14:00) Pacific/Kiritimati",
	  "(GMT+14:00) Pacific/Tongatapu",
	  "(GMT+14:00) Etc/GMT-14",
	  "(GMT+13:45) NZ-CHAT",
	  "(GMT+13:45) Pacific/Chatham",
	  "(GMT+13:00) Pacific/Fakaofo",
	  "(GMT+13:00) Antarctica/McMurdo",
	  "(GMT+13:00) Pacific/Fiji",
	  "(GMT+13:00) Pacific/Enderbury",
	  "(GMT+13:00) NZ",
	  "(GMT+13:00) Antarctica/South_Pole",
	  "(GMT+13:00) Pacific/Auckland",
	  "(GMT+13:00) Etc/GMT-13",
	  "(GMT+12:00) Pacific/Kwajalein",
	  "(GMT+12:00) Pacific/Wallis",
	  "(GMT+12:00) Pacific/Funafuti",
	  "(GMT+12:00) Pacific/Nauru",
	  "(GMT+12:00) Kwajalein",
	  "(GMT+12:00) Pacific/Wake",
	  "(GMT+12:00) Pacific/Tarawa",
	  "(GMT+12:00) Asia/Kamchatka",
	  "(GMT+12:00) Etc/GMT-12",
	  "(GMT+12:00) Asia/Anadyr",
	  "(GMT+12:00) Pacific/Majuro",
	  "(GMT+11:00) Australia/Hobart",
	  "(GMT+11:00) Australia/Tasmania",
	  "(GMT+11:00) Australia/ACT",
	  "(GMT+11:00) Pacific/Ponape",
	  "(GMT+11:00) Pacific/Bougainville",
	  "(GMT+11:00) Australia/Victoria",
	  "(GMT+11:00) Antarctica/Macquarie",
	  "(GMT+11:00) Antarctica/Casey",
	  "(GMT+11:00) Australia/Canberra",
	  "(GMT+11:00) Australia/Currie",
	  "(GMT+11:00) Australia/Lord_Howe",
	  "(GMT+11:00) Australia/NSW",
	  "(GMT+11:00) Pacific/Pohnpei",
	  "(GMT+11:00) Pacific/Efate",
	  "(GMT+11:00) Pacific/Norfolk",
	  "(GMT+11:00) Asia/Magadan",
	  "(GMT+11:00) Pacific/Kosrae",
	  "(GMT+11:00) Australia/Sydney",
	  "(GMT+11:00) Australia/LHI",
	  "(GMT+11:00) Asia/Sakhalin",
	  "(GMT+11:00) Pacific/Noumea",
	  "(GMT+11:00) Etc/GMT-11",
	  "(GMT+11:00) Asia/Srednekolymsk",
	  "(GMT+11:00) Australia/Melbourne",
	  "(GMT+11:00) Pacific/Guadalcanal",
	  "(GMT+10:30) Australia/Yancowinna",
	  "(GMT+10:30) Australia/Adelaide",
	  "(GMT+10:30) Australia/Broken_Hill",
	  "(GMT+10:30) Australia/South",
	  "(GMT+10:00) Pacific/Yap",
	  "(GMT+10:00) Pacific/Port_Moresby",
	  "(GMT+10:00) Pacific/Chuuk",
	  "(GMT+10:00) Australia/Queensland",
	  "(GMT+10:00) Pacific/Guam",
	  "(GMT+10:00) Pacific/Truk",
	  "(GMT+10:00) Asia/Vladivostok",
	  "(GMT+10:00) Pacific/Saipan",
	  "(GMT+10:00) Antarctica/DumontDUrville",
	  "(GMT+10:00) Australia/Brisbane",
	  "(GMT+10:00) Etc/GMT-10",
	  "(GMT+10:00) Asia/Ust-Nera",
	  "(GMT+10:00) Australia/Lindeman",
	  "(GMT+09:30) Australia/North",
	  "(GMT+09:30) Australia/Darwin",
	  "(GMT+09:00) Etc/GMT-9",
	  "(GMT+09:00) Pacific/Palau",
	  "(GMT+09:00) Asia/Chita",
	  "(GMT+09:00) Asia/Dili",
	  "(GMT+09:00) Asia/Jayapura",
	  "(GMT+09:00) Asia/Yakutsk",
	  "(GMT+09:00) ROK",
	  "(GMT+09:00) Asia/Seoul",
	  "(GMT+09:00) Asia/Khandyga",
	  "(GMT+09:00) Japan",
	  "(GMT+09:00) Asia/Tokyo",
	  "(GMT+08:45) Australia/Eucla",
	  "(GMT+08:30) Asia/Pyongyang",
	  "(GMT+08:00) Asia/Kuching",
	  "(GMT+08:00) Asia/Chungking",
	  "(GMT+08:00) Etc/GMT-8",
	  "(GMT+08:00) Australia/Perth",
	  "(GMT+08:00) Asia/Macao",
	  "(GMT+08:00) Asia/Macau",
	  "(GMT+08:00) Asia/Choibalsan",
	  "(GMT+08:00) Asia/Shanghai",
	  "(GMT+08:00) Asia/Ulan_Bator",
	  "(GMT+08:00) Asia/Chongqing",
	  "(GMT+08:00) Asia/Ulaanbaatar",
	  "(GMT+08:00) Asia/Taipei",
	  "(GMT+08:00) Asia/Manila",
	  "(GMT+08:00) PRC",
	  "(GMT+08:00) Asia/Ujung_Pandang",
	  "(GMT+08:00) Asia/Harbin",
	  "(GMT+08:00) Singapore",
	  "(GMT+08:00) Asia/Brunei",
	  "(GMT+08:00) Australia/West",
	  "(GMT+08:00) Asia/Hong_Kong",
	  "(GMT+08:00) Asia/Makassar",
	  "(GMT+08:00) Hongkong",
	  "(GMT+08:00) Asia/Kuala_Lumpur",
	  "(GMT+08:00) Asia/Irkutsk",
	  "(GMT+08:00) Asia/Singapore",
	  "(GMT+07:00) Asia/Pontianak",
	  "(GMT+07:00) Etc/GMT-7",
	  "(GMT+07:00) Asia/Phnom_Penh",
	  "(GMT+07:00) Asia/Novosibirsk",
	  "(GMT+07:00) Antarctica/Davis",
	  "(GMT+07:00) Asia/Tomsk",
	  "(GMT+07:00) Asia/Jakarta",
	  "(GMT+07:00) Asia/Barnaul",
	  "(GMT+07:00) Indian/Christmas",
	  "(GMT+07:00) Asia/Ho_Chi_Minh",
	  "(GMT+07:00) Asia/Hovd",
	  "(GMT+07:00) Asia/Bangkok",
	  "(GMT+07:00) Asia/Vientiane",
	  "(GMT+07:00) Asia/Novokuznetsk",
	  "(GMT+07:00) Asia/Krasnoyarsk",
	  "(GMT+07:00) Asia/Saigon",
	  "(GMT+06:30) Asia/Yangon",
	  "(GMT+06:30) Asia/Rangoon",
	  "(GMT+06:30) Indian/Cocos",
	  "(GMT+06:00) Asia/Kashgar",
	  "(GMT+06:00) Etc/GMT-6",
	  "(GMT+06:00) Asia/Almaty",
	  "(GMT+06:00) Asia/Dacca",
	  "(GMT+06:00) Asia/Omsk",
	  "(GMT+06:00) Asia/Dhaka",
	  "(GMT+06:00) Indian/Chagos",
	  "(GMT+06:00) Asia/Qyzylorda",
	  "(GMT+06:00) Asia/Bishkek",
	  "(GMT+06:00) Antarctica/Vostok",
	  "(GMT+06:00) Asia/Urumqi",
	  "(GMT+06:00) Asia/Thimbu",
	  "(GMT+06:00) Asia/Thimphu",
	  "(GMT+05:45) Asia/Kathmandu",
	  "(GMT+05:45) Asia/Katmandu",
	  "(GMT+05:30) Asia/Kolkata",
	  "(GMT+05:30) Asia/Colombo",
	  "(GMT+05:30) Asia/Calcutta",
	  "(GMT+05:00) Asia/Aqtau",
	  "(GMT+05:00) Etc/GMT-5",
	  "(GMT+05:00) Asia/Samarkand",
	  "(GMT+05:00) Asia/Karachi",
	  "(GMT+05:00) Asia/Yekaterinburg",
	  "(GMT+05:00) Asia/Dushanbe",
	  "(GMT+05:00) Indian/Maldives",
	  "(GMT+05:00) Asia/Oral",
	  "(GMT+05:00) Asia/Tashkent",
	  "(GMT+05:00) Antarctica/Mawson",
	  "(GMT+05:00) Asia/Aqtobe",
	  "(GMT+05:00) Asia/Ashkhabad",
	  "(GMT+05:00) Asia/Ashgabat",
	  "(GMT+05:00) Asia/Atyrau",
	  "(GMT+05:00) Indian/Kerguelen",
	  "(GMT+04:30) Asia/Kabul",
	  "(GMT+04:00) Asia/Yerevan",
	  "(GMT+04:00) Etc/GMT-4",
	  "(GMT+04:00) Asia/Dubai",
	  "(GMT+04:00) Indian/Reunion",
	  "(GMT+04:00) Indian/Mauritius",
	  "(GMT+04:00) Europe/Saratov",
	  "(GMT+04:00) Europe/Samara",
	  "(GMT+04:00) Indian/Mahe",
	  "(GMT+04:00) Asia/Baku",
	  "(GMT+04:00) Asia/Muscat",
	  "(GMT+04:00) Europe/Astrakhan",
	  "(GMT+04:00) Asia/Tbilisi",
	  "(GMT+04:00) Europe/Ulyanovsk",
	  "(GMT+03:30) Iran",
	  "(GMT+03:30) Asia/Tehran",
	  "(GMT+03:00) Asia/Aden",
	  "(GMT+03:00) Africa/Nairobi",
	  "(GMT+03:00) Europe/Istanbul",
	  "(GMT+03:00) Etc/GMT-3",
	  "(GMT+03:00) Indian/Comoro",
	  "(GMT+03:00) Antarctica/Syowa",
	  "(GMT+03:00) Africa/Mogadishu",
	  "(GMT+03:00) Africa/Asmera",
	  "(GMT+03:00) Asia/Istanbul",
	  "(GMT+03:00) Europe/Moscow",
	  "(GMT+03:00) Africa/Djibouti",
	  "(GMT+03:00) Europe/Simferopol",
	  "(GMT+03:00) Africa/Asmara",
	  "(GMT+03:00) Asia/Baghdad",
	  "(GMT+03:00) Africa/Dar_es_Salaam",
	  "(GMT+03:00) Africa/Addis_Ababa",
	  "(GMT+03:00) Asia/Riyadh",
	  "(GMT+03:00) Asia/Kuwait",
	  "(GMT+03:00) Europe/Kirov",
	  "(GMT+03:00) Africa/Kampala",
	  "(GMT+03:00) Europe/Minsk",
	  "(GMT+03:00) Asia/Qatar",
	  "(GMT+03:00) Asia/Bahrain",
	  "(GMT+03:00) Indian/Antananarivo",
	  "(GMT+03:00) Indian/Mayotte",
	  "(GMT+03:00) Europe/Volgograd",
	  "(GMT+03:00) Turkey",
	  "(GMT+03:00) Africa/Khartoum",
	  "(GMT+03:00) Africa/Juba",
	  "(GMT+03:00) Asia/Famagusta",
	  "(GMT+03:00) W-SU",
	  "(GMT+02:00) Africa/Cairo",
	  "(GMT+02:00) Africa/Mbabane",
	  "(GMT+02:00) Etc/GMT-2",
	  "(GMT+02:00) Europe/Zaporozhye",
	  "(GMT+02:00) Libya",
	  "(GMT+02:00) Africa/Kigali",
	  "(GMT+02:00) Africa/Tripoli",
	  "(GMT+02:00) Israel",
	  "(GMT+02:00) Europe/Kaliningrad",
	  "(GMT+02:00) Africa/Windhoek",
	  "(GMT+02:00) Europe/Bucharest",
	  "(GMT+02:00) Europe/Mariehamn",
	  "(GMT+02:00) Africa/Lubumbashi",
	  "(GMT+02:00) Europe/Tiraspol",
	  "(GMT+02:00) Europe/Chisinau",
	  "(GMT+02:00) Europe/Helsinki",
	  "(GMT+02:00) Asia/Beirut",
	  "(GMT+02:00) Asia/Tel_Aviv",
	  "(GMT+02:00) Europe/Sofia",
	  "(GMT+02:00) Africa/Gaborone",
	  "(GMT+02:00) Asia/Gaza",
	  "(GMT+02:00) Europe/Riga",
	  "(GMT+02:00) Africa/Maputo",
	  "(GMT+02:00) Asia/Damascus",
	  "(GMT+02:00) Europe/Uzhgorod",
	  "(GMT+02:00) Asia/Jerusalem",
	  "(GMT+02:00) Africa/Bujumbura",
	  "(GMT+02:00) Europe/Kiev",
	  "(GMT+02:00) Europe/Vilnius",
	  "(GMT+02:00) Africa/Maseru",
	  "(GMT+02:00) Africa/Blantyre",
	  "(GMT+02:00) Africa/Lusaka",
	  "(GMT+02:00) Africa/Harare",
	  "(GMT+02:00) Europe/Tallinn",
	  "(GMT+02:00) Africa/Johannesburg",
	  "(GMT+02:00) Asia/Nicosia",
	  "(GMT+02:00) EET",
	  "(GMT+02:00) Asia/Hebron",
	  "(GMT+02:00) Egypt",
	  "(GMT+02:00) Asia/Amman",
	  "(GMT+02:00) Europe/Nicosia",
	  "(GMT+02:00) Europe/Athens",
	  "(GMT+01:00) Europe/Brussels",
	  "(GMT+01:00) Europe/Warsaw",
	  "(GMT+01:00) CET",
	  "(GMT+01:00) Etc/GMT-1",
	  "(GMT+01:00) Europe/Luxembourg",
	  "(GMT+01:00) Africa/Tunis",
	  "(GMT+01:00) Europe/Malta",
	  "(GMT+01:00) Europe/Busingen",
	  "(GMT+01:00) Africa/Malabo",
	  "(GMT+01:00) Europe/Skopje",
	  "(GMT+01:00) Europe/Sarajevo",
	  "(GMT+01:00) Africa/Lagos",
	  "(GMT+01:00) Europe/Rome",
	  "(GMT+01:00) Africa/Algiers",
	  "(GMT+01:00) Europe/Zurich",
	  "(GMT+01:00) Europe/Gibraltar",
	  "(GMT+01:00) Europe/Vaduz",
	  "(GMT+01:00) Europe/Ljubljana",
	  "(GMT+01:00) Europe/Berlin",
	  "(GMT+01:00) Europe/Stockholm",
	  "(GMT+01:00) Europe/Budapest",
	  "(GMT+01:00) Europe/Zagreb",
	  "(GMT+01:00) Europe/Paris",
	  "(GMT+01:00) Africa/Ndjamena",
	  "(GMT+01:00) Africa/Ceuta",
	  "(GMT+01:00) Europe/Prague",
	  "(GMT+01:00) Europe/Copenhagen",
	  "(GMT+01:00) Europe/Vienna",
	  "(GMT+01:00) Europe/Tirane",
	  "(GMT+01:00) MET",
	  "(GMT+01:00) Europe/Amsterdam",
	  "(GMT+01:00) Africa/Libreville",
	  "(GMT+01:00) Europe/San_Marino",
	  "(GMT+01:00) Africa/Douala",
	  "(GMT+01:00) Africa/Brazzaville",
	  "(GMT+01:00) Africa/Porto-Novo",
	  "(GMT+01:00) Poland",
	  "(GMT+01:00) Europe/Andorra",
	  "(GMT+01:00) Europe/Oslo",
	  "(GMT+01:00) Europe/Podgorica",
	  "(GMT+01:00) Africa/Luanda",
	  "(GMT+01:00) Atlantic/Jan_Mayen",
	  "(GMT+01:00) Africa/Kinshasa",
	  "(GMT+01:00) Europe/Madrid",
	  "(GMT+01:00) Africa/Bangui",
	  "(GMT+01:00) Europe/Belgrade",
	  "(GMT+01:00) Africa/Niamey",
	  "(GMT+01:00) Europe/Bratislava",
	  "(GMT+01:00) Arctic/Longyearbyen",
	  "(GMT+01:00) Europe/Vatican",
	  "(GMT+01:00) Europe/Monaco",
	  "(GMTZ) Europe/London",
	  "(GMTZ) GMT",
	  "(GMTZ) Etc/GMT-0",
	  "(GMTZ) Europe/Jersey",
	  "(GMTZ) Atlantic/St_Helena",
	  "(GMTZ) Europe/Guernsey",
	  "(GMTZ) Europe/Isle_of_Man",
	  "(GMTZ) Etc/GMT+0",
	  "(GMTZ) Africa/Banjul",
	  "(GMTZ) Etc/GMT",
	  "(GMTZ) Africa/Freetown",
	  "(GMTZ) GB-Eire",
	  "(GMTZ) Africa/Bamako",
	  "(GMTZ) GB",
	  "(GMTZ) Africa/Conakry",
	  "(GMTZ) Portugal",
	  "(GMTZ) Universal",
	  "(GMTZ) Africa/Sao_Tome",
	  "(GMTZ) Africa/Nouakchott",
	  "(GMTZ) Antarctica/Troll",
	  "(GMTZ) UTC",
	  "(GMTZ) Etc/Universal",
	  "(GMTZ) Atlantic/Faeroe",
	  "(GMTZ) Africa/Abidjan",
	  "(GMTZ) Eire",
	  "(GMTZ) Africa/Accra",
	  "(GMTZ) Atlantic/Faroe",
	  "(GMTZ) Etc/UCT",
	  "(GMTZ) GMT0",
	  "(GMTZ) Europe/Dublin",
	  "(GMTZ) Zulu",
	  "(GMTZ) Africa/El_Aaiun",
	  "(GMTZ) Africa/Ouagadougou",
	  "(GMTZ) Atlantic/Reykjavik",
	  "(GMTZ) Atlantic/Madeira",
	  "(GMTZ) Etc/Zulu",
	  "(GMTZ) Iceland",
	  "(GMTZ) Europe/Lisbon",
	  "(GMTZ) Atlantic/Canary",
	  "(GMTZ) Africa/Lome",
	  "(GMTZ) Greenwich",
	  "(GMTZ) Africa/Casablanca",
	  "(GMTZ) Europe/Belfast",
	  "(GMTZ) Etc/GMT0",
	  "(GMTZ) America/Danmarkshavn",
	  "(GMTZ) Africa/Dakar",
	  "(GMTZ) Africa/Bissau",
	  "(GMTZ) WET",
	  "(GMTZ) Etc/Greenwich",
	  "(GMTZ) Africa/Timbuktu",
	  "(GMTZ) UCT",
	  "(GMTZ) Africa/Monrovia",
	  "(GMTZ) Etc/UTC",
	  "(GMT-01:00) Etc/GMT+1",
	  "(GMT-01:00) Atlantic/Cape_Verde",
	  "(GMT-01:00) Atlantic/Azores",
	  "(GMT-01:00) America/Scoresbysund",
	  "(GMT-02:00) Etc/GMT+2",
	  "(GMT-02:00) Brazil/East",
	  "(GMT-02:00) America/Sao_Paulo",
	  "(GMT-02:00) America/Noronha",
	  "(GMT-02:00) Brazil/DeNoronha",
	  "(GMT-02:00) Atlantic/South_Georgia",
	  "(GMT-03:00) America/Cuiaba",
	  "(GMT-03:00) Chile/Continental",
	  "(GMT-03:00) America/Miquelon",
	  "(GMT-03:00) America/Argentina/Catamarca",
	  "(GMT-03:00) America/Argentina/Cordoba",
	  "(GMT-03:00) America/Araguaina",
	  "(GMT-03:00) America/Argentina/Salta",
	  "(GMT-03:00) Etc/GMT+3",
	  "(GMT-03:00) America/Montevideo",
	  "(GMT-03:00) America/Argentina/Mendoza",
	  "(GMT-03:00) America/Argentina/Rio_Gallegos",
	  "(GMT-03:00) America/Catamarca",
	  "(GMT-03:00) America/Godthab",
	  "(GMT-03:00) America/Cordoba",
	  "(GMT-03:00) America/Argentina/Jujuy",
	  "(GMT-03:00) America/Cayenne",
	  "(GMT-03:00) America/Recife",
	  "(GMT-03:00) America/Buenos_Aires",
	  "(GMT-03:00) America/Paramaribo",
	  "(GMT-03:00) America/Mendoza",
	  "(GMT-03:00) America/Santarem",
	  "(GMT-03:00) America/Asuncion",
	  "(GMT-03:00) America/Maceio",
	  "(GMT-03:00) Atlantic/Stanley",
	  "(GMT-03:00) Antarctica/Rothera",
	  "(GMT-03:00) America/Argentina/San_Luis",
	  "(GMT-03:00) America/Santiago",
	  "(GMT-03:00) America/Argentina/Ushuaia",
	  "(GMT-03:00) Antarctica/Palmer",
	  "(GMT-03:00) America/Punta_Arenas",
	  "(GMT-03:00) America/Fortaleza",
	  "(GMT-03:00) America/Argentina/La_Rioja",
	  "(GMT-03:00) America/Campo_Grande",
	  "(GMT-03:00) America/Belem",
	  "(GMT-03:00) America/Jujuy",
	  "(GMT-03:00) America/Bahia",
	  "(GMT-03:00) America/Argentina/San_Juan",
	  "(GMT-03:00) America/Argentina/Tucuman",
	  "(GMT-03:00) America/Rosario",
	  "(GMT-03:00) America/Argentina/Buenos_Aires",
	  "(GMT-03:30) America/St_Johns",
	  "(GMT-03:30) Canada/Newfoundland",
	  "(GMT-04:00) America/Marigot",
	  "(GMT-04:00) Canada/Atlantic",
	  "(GMT-04:00) America/Grand_Turk",
	  "(GMT-04:00) Etc/GMT+4",
	  "(GMT-04:00) America/Manaus",
	  "(GMT-04:00) America/St_Thomas",
	  "(GMT-04:00) America/Anguilla",
	  "(GMT-04:00) America/Barbados",
	  "(GMT-04:00) America/Curacao",
	  "(GMT-04:00) America/Guyana",
	  "(GMT-04:00) America/Martinique",
	  "(GMT-04:00) America/Puerto_Rico",
	  "(GMT-04:00) America/Port_of_Spain",
	  "(GMT-04:00) SystemV/AST4",
	  "(GMT-04:00) America/Kralendijk",
	  "(GMT-04:00) America/Antigua",
	  "(GMT-04:00) America/Moncton",
	  "(GMT-04:00) America/St_Vincent",
	  "(GMT-04:00) America/Dominica",
	  "(GMT-04:00) Atlantic/Bermuda",
	  "(GMT-04:00) Brazil/West",
	  "(GMT-04:00) America/Aruba",
	  "(GMT-04:00) America/Halifax",
	  "(GMT-04:00) America/La_Paz",
	  "(GMT-04:00) America/Blanc-Sablon",
	  "(GMT-04:00) America/Santo_Domingo",
	  "(GMT-04:00) America/Glace_Bay",
	  "(GMT-04:00) America/St_Barthelemy",
	  "(GMT-04:00) America/St_Lucia",
	  "(GMT-04:00) America/Montserrat",
	  "(GMT-04:00) America/Lower_Princes",
	  "(GMT-04:00) America/Thule",
	  "(GMT-04:00) America/Tortola",
	  "(GMT-04:00) America/Porto_Velho",
	  "(GMT-04:00) America/Goose_Bay",
	  "(GMT-04:00) America/Virgin",
	  "(GMT-04:00) America/Boa_Vista",
	  "(GMT-04:00) America/Grenada",
	  "(GMT-04:00) America/St_Kitts",
	  "(GMT-04:00) America/Caracas",
	  "(GMT-04:00) America/Guadeloupe",
	  "(GMT-04:00) SystemV/AST4ADT",
	  "(GMT-05:00) America/Panama",
	  "(GMT-05:00) America/Indiana/Petersburg",
	  "(GMT-05:00) America/Eirunepe",
	  "(GMT-05:00) Cuba",
	  "(GMT-05:00) Etc/GMT+5",
	  "(GMT-05:00) Pacific/Easter",
	  "(GMT-05:00) America/Fort_Wayne",
	  "(GMT-05:00) America/Havana",
	  "(GMT-05:00) America/Porto_Acre",
	  "(GMT-05:00) US/Michigan",
	  "(GMT-05:00) America/Louisville",
	  "(GMT-05:00) America/Guayaquil",
	  "(GMT-05:00) America/Indiana/Vevay",
	  "(GMT-05:00) America/Indiana/Vincennes",
	  "(GMT-05:00) America/Indianapolis",
	  "(GMT-05:00) America/Iqaluit",
	  "(GMT-05:00) America/Kentucky/Louisville",
	  "(GMT-05:00) EST5EDT",
	  "(GMT-05:00) America/Nassau",
	  "(GMT-05:00) America/Jamaica",
	  "(GMT-05:00) America/Atikokan",
	  "(GMT-05:00) America/Kentucky/Monticello",
	  "(GMT-05:00) America/Coral_Harbour",
	  "(GMT-05:00) America/Cayman",
	  "(GMT-05:00) Chile/EasterIsland",
	  "(GMT-05:00) America/Indiana/Indianapolis",
	  "(GMT-05:00) America/Thunder_Bay",
	  "(GMT-05:00) America/Indiana/Marengo",
	  "(GMT-05:00) America/Bogota",
	  "(GMT-05:00) SystemV/EST5",
	  "(GMT-05:00) US/Eastern",
	  "(GMT-05:00) Canada/Eastern",
	  "(GMT-05:00) America/Port-au-Prince",
	  "(GMT-05:00) America/Nipigon",
	  "(GMT-05:00) Brazil/Acre",
	  "(GMT-05:00) US/East-Indiana",
	  "(GMT-05:00) America/Cancun",
	  "(GMT-05:00) America/Lima",
	  "(GMT-05:00) America/Rio_Branco",
	  "(GMT-05:00) America/Detroit",
	  "(GMT-05:00) Jamaica",
	  "(GMT-05:00) America/Pangnirtung",
	  "(GMT-05:00) America/Montreal",
	  "(GMT-05:00) America/Indiana/Winamac",
	  "(GMT-05:00) America/New_York",
	  "(GMT-05:00) America/Toronto",
	  "(GMT-05:00) SystemV/EST5EDT",
	  "(GMT-06:00) America/El_Salvador",
	  "(GMT-06:00) America/Guatemala",
	  "(GMT-06:00) America/Belize",
	  "(GMT-06:00) America/Managua",
	  "(GMT-06:00) America/Chicago",
	  "(GMT-06:00) America/Tegucigalpa",
	  "(GMT-06:00) Etc/GMT+6",
	  "(GMT-06:00) America/Regina",
	  "(GMT-06:00) Mexico/General",
	  "(GMT-06:00) America/Rankin_Inlet",
	  "(GMT-06:00) US/Central",
	  "(GMT-06:00) Pacific/Galapagos",
	  "(GMT-06:00) America/Rainy_River",
	  "(GMT-06:00) America/Swift_Current",
	  "(GMT-06:00) America/Costa_Rica",
	  "(GMT-06:00) America/Indiana/Knox",
	  "(GMT-06:00) America/North_Dakota/Beulah",
	  "(GMT-06:00) Canada/East-Saskatchewan",
	  "(GMT-06:00) America/Monterrey",
	  "(GMT-06:00) SystemV/CST6",
	  "(GMT-06:00) America/North_Dakota/Center",
	  "(GMT-06:00) America/Indiana/Tell_City",
	  "(GMT-06:00) America/Mexico_City",
	  "(GMT-06:00) America/Matamoros",
	  "(GMT-06:00) CST6CDT",
	  "(GMT-06:00) America/Knox_IN",
	  "(GMT-06:00) America/Menominee",
	  "(GMT-06:00) America/Resolute",
	  "(GMT-06:00) Canada/Central",
	  "(GMT-06:00) America/Bahia_Banderas",
	  "(GMT-06:00) US/Indiana-Starke",
	  "(GMT-06:00) SystemV/CST6CDT",
	  "(GMT-06:00) America/Merida",
	  "(GMT-06:00) Canada/Saskatchewan",
	  "(GMT-06:00) America/Winnipeg",
	  "(GMT-07:00) Etc/GMT+7",
	  "(GMT-07:00) US/Arizona",
	  "(GMT-07:00) Mexico/BajaSur",
	  "(GMT-07:00) America/Dawson_Creek",
	  "(GMT-07:00) America/Denver",
	  "(GMT-07:00) America/Yellowknife",
	  "(GMT-07:00) America/Inuvik",
	  "(GMT-07:00) America/Mazatlan",
	  "(GMT-07:00) SystemV/MST7",
	  "(GMT-07:00) America/Boise",
	  "(GMT-07:00) MST7MDT",
	  "(GMT-07:00) America/Chihuahua",
	  "(GMT-07:00) America/Ojinaga",
	  "(GMT-07:00) US/Mountain",
	  "(GMT-07:00) America/Creston",
	  "(GMT-07:00) America/Edmonton",
	  "(GMT-07:00) America/Hermosillo",
	  "(GMT-07:00) Canada/Mountain",
	  "(GMT-07:00) America/Cambridge_Bay",
	  "(GMT-07:00) Navajo",
	  "(GMT-07:00) America/Phoenix",
	  "(GMT-07:00) SystemV/MST7MDT",
	  "(GMT-07:00) America/Fort_Nelson",
	  "(GMT-07:00) America/Shiprock",
	  "(GMT-08:00) Etc/GMT+8",
	  "(GMT-08:00) Canada/Yukon",
	  "(GMT-08:00) US/Pacific-New",
	  "(GMT-08:00) Canada/Pacific",
	  "(GMT-08:00) PST8PDT",
	  "(GMT-08:00) Pacific/Pitcairn",
	  "(GMT-08:00) America/Dawson",
	  "(GMT-08:00) Mexico/BajaNorte",
	  "(GMT-08:00) America/Tijuana",
	  "(GMT-08:00) SystemV/PST8",
	  "(GMT-08:00) America/Santa_Isabel",
	  "(GMT-08:00) America/Vancouver",
	  "(GMT-08:00) America/Ensenada",
	  "(GMT-08:00) America/Whitehorse",
	  "(GMT-08:00) SystemV/PST8PDT",
	  "(GMT-08:00) America/Los_Angeles",
	  "(GMT-08:00) US/Pacific",
	  "(GMT-09:00) Etc/GMT+9",
	  "(GMT-09:00) US/Alaska",
	  "(GMT-09:00) America/Juneau",
	  "(GMT-09:00) America/Metlakatla",
	  "(GMT-09:00) Pacific/Gambier",
	  "(GMT-09:00) America/Yakutat",
	  "(GMT-09:00) America/Sitka",
	  "(GMT-09:00) SystemV/YST9",
	  "(GMT-09:00) America/Anchorage",
	  "(GMT-09:00) America/Nome",
	  "(GMT-09:00) SystemV/YST9YDT",
	  "(GMT-09:30) Pacific/Marquesas",
	  "(GMT-10:00) Pacific/Honolulu",
	  "(GMT-10:00) Pacific/Rarotonga",
	  "(GMT-10:00) Pacific/Tahiti",
	  "(GMT-10:00) Pacific/Johnston",
	  "(GMT-10:00) America/Atka",
	  "(GMT-10:00) US/Hawaii",
	  "(GMT-10:00) SystemV/HST10",
	  "(GMT-10:00) America/Adak",
	  "(GMT-10:00) US/Aleutian",
	  "(GMT-10:00) Etc/GMT+10",
	  "(GMT-11:00) Pacific/Pago_Pago",
	  "(GMT-11:00) Pacific/Samoa",
	  "(GMT-11:00) Pacific/Niue",
	  "(GMT-11:00) US/Samoa",
	  "(GMT-11:00) Etc/GMT+11",
	  "(GMT-11:00) Pacific/Midway",
	  "(GMT-12:00) Etc/GMT+12"
	];
	var srcOfWeb = document.location.href;
	var obj = {

	    name: "ИКБО-04-16",
	    dateStart: "09/01/2018",
	    dateFinish: "11/30/2018",
	    timeZone: "(GMT+03:00) Europe/Moscow"
	};

  	$(function(){
    	$('#datepickerStart').datepicker({
    		changeMonth: true,
    		changeYear: true
   		});
  	});
  	$(function(){
    	$('#datepickerFinish').datepicker({
			changeMonth: true,
    		changeYear: true
    	});
  	});

	for(var i = 0; i < 598; i++){

		$('<div/>', {

			class: 'timezones',
			id: 'zone' + i,
		}).appendTo('#divForTimezones');
		document.getElementById('zone' + i).innerHTML += zonesArr[i];
	}

	$('#timezone').on('click', function(){
		
		if(flagForTimezones == false){

			$('#divForTimezones').fadeIn();
			$('#supportDivForBottomBorder').fadeIn();
			flagForTimezones = true;
		}else{

			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			flagForTimezones = false;
		}
	});

	$('.timezones').on('click', function(){

		var id = $(this).attr('id');
		id = id.replace('zone', '');
		$('#timezone').attr('value', zonesArr[id]);
		$('#divForTimezones').fadeOut();
		$('#supportDivForBottomBorder').fadeOut();
		flagForTimezones = false;
	});

	$('.cogSettings').on('click', function(){

		if(flagForSettings == false){

			$('#wrapperForSettings').slideDown();
			flagForSettings = true;
		}else{
			$('#wrapperForSettings').slideUp();
			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			flagForSettings = false;
			flagForTimezones = false;
		}
	});

	$('#getButton').on('click', function(){

		if(flagForSettings == true){

			$('#wrapperForSettings').slideUp();
			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			flagForSettings = false;
			flagForTimezones = false;
		}

		obj.name = document.getElementById("inputFIO").value;
		obj.dateStart = document.getElementById("datepickerStart").value;
		obj.dateFinish = document.getElementById("datepickerFinish").value;
		obj.timeZone = document.getElementById("timezone").value;

		obj.dateStart = obj.dateStart.charAt(6) + obj.dateStart.charAt(7) + obj.dateStart.charAt(8) + obj.dateStart.charAt(9) + '-' + obj.dateStart.charAt(0) + obj.dateStart.charAt(1) + '-' + obj.dateStart.charAt(3) + obj.dateStart.charAt(4);
		obj.dateFinish = obj.dateFinish.charAt(6) + obj.dateFinish.charAt(7) + obj.dateFinish.charAt(8) + obj.dateFinish.charAt(9) + '-' + obj.dateFinish.charAt(0) + obj.dateFinish.charAt(1) + '-' + obj.dateFinish.charAt(3) + obj.dateFinish.charAt(4);
		if(obj.timeZone.charAt(4) == 'Z'){

			obj.timeZone = obj.timeZone.replace('(GMTZ) ', '');
		}else{

			let tmp = obj.timeZone.charAt(0) + obj.timeZone.charAt(1) + obj.timeZone.charAt(2) + obj.timeZone.charAt(3) + obj.timeZone.charAt(4) + obj.timeZone.charAt(5) + obj.timeZone.charAt(6) + obj.timeZone.charAt(7) + obj.timeZone.charAt(8) + obj.timeZone.charAt(9) + obj.timeZone.charAt(10) + obj.timeZone.charAt(11);
			obj.timeZone = obj.timeZone.replace(tmp, '');
		}
		var srcForInput = document.location.href + 'schedule?name=' + encodeURIComponent(obj.name) + '&dateStart=' + obj.dateStart + '&dateFinish=' + obj.dateFinish + '&timezoneStart=' + encodeURIComponent(obj.timeZone);

		$('#copyDiv').attr('value', srcForInput);
		$('#wrapperForSecondModal').fadeIn();
	});

	$('#flexibleImage').on('click', function(){

		if(flagForDiscription == false){

			$('#wrapperForFirstModal').fadeIn();
			$('#wrapperForFirstModal').css('overflow', 'hidden');
			flagForDiscription = true;
		}
	});

	$('#wrapperForFirstModal').on('click', function(){

		if(flagForDiscription == true){

			$('#wrapperForFirstModal').fadeOut();
			$('#wrapperForFirstModal').css('overflow', 'auto');
			flagForDiscription = false;
		}
	});

	$('#close').on('click', function(){

		$('#wrapperForSecondModal').fadeOut();
		$('#wrapperForSecondModal').css('overflow', 'auto');
	});

	$('#buttonForCopy').on('click', function(){

		$('#copyDiv').execCommand('copy');
	});


	document.getElementById('buttonForCopy').addEventListener('click', function(){

	    copyToClipboard(document.getElementById('copyDiv'));
	});

	function copyToClipboard(elem){

	    var targetId = '_hiddenCopyText_';
	    var isInput = elem.tagName === 'INPUT' || elem.tagName === 'TEXTAREA';
	    var origSelectionStart, origSelectionEnd;
	    if(isInput){
	    	
	        target = elem;
	        origSelectionStart = elem.selectionStart;
	        origSelectionEnd = elem.selectionEnd;
	    }else{

	        target = document.getElementById(targetId);
	        if(!target){
	            var target = document.createElement('textarea');
	            target.style.position = 'absolute';
	            target.style.left = '-9999px';
	            target.style.top = '0';
	            target.id = targetId;
	            document.body.appendChild(target);
	        }
	        target.textContent = elem.textContent;
	    }

	    var currentFocus = document.activeElement;
	    target.focus();
	    target.setSelectionRange(0, target.value.length);
	    var succeed;
	    try{
	    	succeed = document.execCommand('copy');
	    }catch(e){
	        succeed = false;
	    }
	    if(currentFocus && typeof currentFocus.focus === 'function'){
	    	currentFocus.focus();
	    }
	    if(isInput){
	        elem.setSelectionRange(origSelectionStart, origSelectionEnd);
	    }else{
	        target.textContent = '';
	    }
	    return succeed;
	}
});