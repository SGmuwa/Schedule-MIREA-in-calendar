package ru.mirea.xlsical.backend.repository;

import org.springframework.data.repository.CrudRepository;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;

public interface StatusRepository extends CrudRepository<ScheduleStatus, Long> {
}
