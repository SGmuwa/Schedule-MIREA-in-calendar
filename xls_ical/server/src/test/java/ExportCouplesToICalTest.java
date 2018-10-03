import net.fortuna.ical4j.model.*;
import net.fortuna.ical4j.model.component.VEvent;
import org.junit.Test;

import static org.junit.Assert.*;

public class ExportCouplesToICalTest {

    @Test
    public void Tutorial(){
        TimeZoneRegistry registry = TimeZoneRegistryFactory.getInstance().createRegistry();
        TimeZone timezone = registry.getTimeZone("Australia/Melbourne");

        java.util.Calendar cal = java.util.Calendar.getInstance(timezone);
        cal.set(java.util.Calendar.YEAR, 2005);
        cal.set(java.util.Calendar.MONTH, java.util.Calendar.NOVEMBER);
        cal.set(java.util.Calendar.DAY_OF_MONTH, 1);
        cal.set(java.util.Calendar.HOUR_OF_DAY, 15);
        cal.clear(java.util.Calendar.MINUTE);
        cal.clear(java.util.Calendar.SECOND);

        DateTime dt = new DateTime(cal.getTime());
        dt.setTimeZone(timezone);
        VEvent melbourneCup = new VEvent(dt, "Melbourne Cup");

        String[] result = melbourneCup.toString().split("\r\n");

        assertNotNull(result);
        assertEquals(5, result.length);
        assertEquals("BEGIN:VEVENT", result[0]);
        assertEquals("DTSTART;TZID=Australia/Melbourne:20051101T150000", result[2]);
        assertEquals("SUMMARY:Melbourne Cup", result[3]);
        assertEquals("END:VEVENT", result[4]);
    }
}
