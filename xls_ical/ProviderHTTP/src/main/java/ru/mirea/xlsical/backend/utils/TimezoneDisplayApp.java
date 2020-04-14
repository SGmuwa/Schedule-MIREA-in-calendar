/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Artemy Mikhailovich Urodovskikh

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

package ru.mirea.xlsical.backend.utils;

import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.util.Comparator;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;


// взято из https://www.baeldung.com/java-time-zones
public class TimezoneDisplayApp {

    public enum OffsetBase {
        GMT, UTC
    }

    Set<String> availableZoneIds = ZoneId.getAvailableZoneIds();

    public List<String> getTimeZoneList(OffsetBase base) {

        LocalDateTime now = LocalDateTime.now();
        return ZoneId.getAvailableZoneIds().stream()
                .map(ZoneId::of)
                .sorted(new ZoneComparator())
                .map(id -> String.format(
                        "(%s%s) %s",
                        base, getOffset(now, id), id.getId()))
                .collect(Collectors.toList());
    }

    private String getOffset(LocalDateTime dateTime, ZoneId id) {
        return dateTime
                .atZone(id)
                .getOffset()
                .getId();
    }

    private class ZoneComparator implements Comparator<ZoneId> {

        @Override
        public int compare(ZoneId zoneId1, ZoneId zoneId2) {
            LocalDateTime now = LocalDateTime.now();
            ZoneOffset offset1 = now.atZone(zoneId1).getOffset();
            ZoneOffset offset2 = now.atZone(zoneId2).getOffset();

            return offset1.compareTo(offset2);
        }
    }

}
