package ru.mirea.xlsical.backend.service;

import org.springframework.stereotype.Service;

@Service("dataService")
public class DataServiceImpl implements DataService {

//    private static final Logger LOG = LoggerFactory.getLogger(DataServiceImpl.class);

//    @Autowired
//    @Qualifier("dataRespitory")
//    private DataRepository dataRepository;

    @Override
    public boolean persist(String problem) {
        try {
//            dataRepository.persist(new Data(UUID.randomUUID(), problem));
            return true;
        } catch (Exception e) {
//            LOG.error("ERROR SAVING DATA: " + e.getMessage(), e);
            return false;
        }
    }
//
//    @Override
//    public Set<String> getRandomData() {
//        return dataRepository.getRandomData();
//    }
}