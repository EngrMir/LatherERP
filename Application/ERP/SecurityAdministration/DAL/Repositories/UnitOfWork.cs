using System;

namespace SecurityAdministration.DAL.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed;
        private readonly BLC_DEVEntities _dbContext = new BLC_DEVEntities();

        private DesignationRepository _deesignationRepository;
        private AccessLogRepository _accessLogRepository;
        private AdditionalOperationPermissionRepository _additionalOperationPermissionRepository;
        private AdditionalScreenPermissionRepository _additionalScreenPermissionRepository;
        private ApplicationPolicyRepository _applicationPolicyRepository;
        private ApplicationRepository _applicationRepository;
        private MenuItemRepository _menuItemRepository;
        private ModuleRepository _moduleRepository;
        private RoleRepository _roleRepository;
        private RoleWiseOperationPermissionRepository _rolewiseOperationPermissionRepository;
        private RoleWiseScreenPermissionRepository _rolewiseScreenPermissionRepository;
        private ScreenOperationRepository _screenOperationRepository;
        private ScreenRepository _screenRepository;
        private UserCredentialInformationRepository _userCredentialInformationRepository;
        private UserInRoleRepository _userInRoleRepository;
        private UserRepository _userRepository;

        public DesignationRepository DesignationRepository
        {
            get { return _deesignationRepository ?? (_deesignationRepository = new DesignationRepository(_dbContext)); }
        }

        public AccessLogRepository AccessLogRepository
        {
            get { return _accessLogRepository ?? (_accessLogRepository = new AccessLogRepository(_dbContext)); }
        }

        public AdditionalOperationPermissionRepository AdditionalOperationPermissionRepository
        {
            get
            {
                return _additionalOperationPermissionRepository ?? (_additionalOperationPermissionRepository = new AdditionalOperationPermissionRepository(_dbContext));
            }
        }

        public AdditionalScreenPermissionRepository AdditionalScreenPermissionRepository
        {
            get { return _additionalScreenPermissionRepository ?? (_additionalScreenPermissionRepository = new AdditionalScreenPermissionRepository(_dbContext)); }
        }

        public ApplicationPolicyRepository ApplicationPolicyRepository
        {
            get { return _applicationPolicyRepository ?? (_applicationPolicyRepository = new ApplicationPolicyRepository(_dbContext)); }
        }

        public ApplicationRepository ApplicationRepository
        {
            get { return _applicationRepository ?? (_applicationRepository = new ApplicationRepository(_dbContext)); }
        }

        public MenuItemRepository MenuItemRepository
        {
            get { return _menuItemRepository ?? (_menuItemRepository = new MenuItemRepository(_dbContext)); }
        }

        public ModuleRepository ModuleRepository
        {
            get { return _moduleRepository ?? (_moduleRepository = new ModuleRepository(_dbContext)); }
        }

        public RoleRepository RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository = new RoleRepository(_dbContext)); }
        }

        public RoleWiseOperationPermissionRepository RolewiseOperationPermissionRepository
        {
            get { return _rolewiseOperationPermissionRepository ?? (_rolewiseOperationPermissionRepository = new RoleWiseOperationPermissionRepository(_dbContext)); }
        }

        public RoleWiseScreenPermissionRepository RolewiseScreenPermissionRepository
        {
            get { return _rolewiseScreenPermissionRepository ?? (_rolewiseScreenPermissionRepository = new RoleWiseScreenPermissionRepository(_dbContext)); }
        }

        public ScreenOperationRepository ScreenOperationRepository
        {
            get { return _screenOperationRepository ?? (_screenOperationRepository = new ScreenOperationRepository(_dbContext)); }
        }

        public ScreenRepository ScreenRepository
        {
            get { return _screenRepository ?? (_screenRepository = new ScreenRepository(_dbContext)); }
        }

        public UserCredentialInformationRepository UserCredentialInformationRepository
        {
            get { return _userCredentialInformationRepository ?? (_userCredentialInformationRepository = new UserCredentialInformationRepository(_dbContext)); }
        }

        public UserInRoleRepository UserInRoleRepository
        {
            get { return _userInRoleRepository ?? (_userInRoleRepository = new UserInRoleRepository(_dbContext)); }
        }

        public UserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new UserRepository(_dbContext)); }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}