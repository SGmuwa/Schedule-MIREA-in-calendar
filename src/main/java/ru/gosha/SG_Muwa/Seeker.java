import java.util.Date;
import java.util.LinkedList;
import java.util.List;

public class Seeker {
    public final String NameOfSeeker;
    public final SeekerType seekerType;
    /**
     * Начало занятий.
     */
    public final Date dateStart;
    public List<Couple> Couples = new LinkedList<>();

    public Seeker(String nameOfSeeker, SeekerType seekerType, Date dateStart) {
        NameOfSeeker = nameOfSeeker;
        this.seekerType = seekerType;
        this.dateStart = dateStart;
    }
}
