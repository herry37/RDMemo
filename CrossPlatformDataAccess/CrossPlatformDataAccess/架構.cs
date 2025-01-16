namespace CrossPlatformDataAccess;

//主架構

//CrossPlatformDataAccess/
//├── Core/
//│   ├── Interfaces/
//│   │   ├── IDbContext.cs
//│   │   ├── IMongoDbContext.cs        
//│   │   ├── IRepository.cs
//│   │   ├── IMongoRepository.cs       
//│   │   └── IUnitOfWork.cs
//├── Infrastructure/
//│   ├── Context/
//│   │   ├── DatabaseContext.cs
//│   │   └── MongoDbContext.cs         
//│   ├── Repositories/
//│   │   ├── GenericRepository.cs
//│   │   └── MongoRepository.cs        
//│   └── UnitOfWork.cs
//└── Common/
//    ├── Enums/
//    │   └── DatabaseType.cs           
//    └── Configuration/
//        ├── DatabaseConfig.cs
//        └── MongoDbConfig.cs          


//# 資料夾結構
//# 在 CrossPlatformDataAccess 專案目錄下建立資料夾
//mkdir Core
//mkdir Core\Interfaces
//mkdir Infrastructure
//mkdir Infrastructure\Context
//mkdir Infrastructure\Repositories
//mkdir Common
//mkdir Common\Enums
//mkdir Common\Configuration