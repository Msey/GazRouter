using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Authorization.Role;
namespace GazRouter.Common
{
    public enum NodeType
    {
        Root = 0,
        Branch,
        Leaf
    }
    public enum LinkType
    {
        Root           = 100,
        // мониторинг
        Monitoring     = 1000,
            Schema         = 1001,
            Kc             = 1002,
            Gpa            = 1003,
            Grs            = 1004,
            Gis            = 1005,
            Urg            = 1006,
            PhysChymParams = 1007,
            ZraLog         = 1008,
            GpaStop        = 1009,
            GasReserve     = 1010,
            Infopanel      = 1011,
        // журнал событий
        EventLog       = 2000,
            Log            = 2001,
        // диспетчерские задания
        DispTasks      = 3000,
            DispTasksAll   = 3001,
            
        // учет газа (балансы)
        Gasaux         = 4000,
            Plan           = 4001,
            DayBalance     = 4002,
            MonthBalance   = 4003,
            GasCosts       = 4004,
            GasOwners      = 4005,
            DistrNetworks  = 4006,
            Routes         = 4008,
            BalanceGroups  = 4009,
            GasCosts2      = 4010,
        // ремонты
        Repairs        = 5000,
                RepairPlan =       5001,
                RepairDva  =       5002,
                RepairRequest =    5003,
                RepairInProgress = 5004,
                RepairComplited =  5005,
                RepairAgreements = 5006,
        // ввод данных
        Input          = 6000,
            Dashboard      = 6001,
            Hourly         = 6002,
            Daily          = 6003,
            SiteInput      = 6004,
            CompUnit       = 6005,
            Valve          = 6006,
            ChemicalTests  = 6007,
            CompUnitTests  = 6008,
            PipelineLimits = 6009,
            ContractPressures = 6010,
        // настройки
        Settings       = 7000,
            Permissions    = 7001,
            ObjectModel    = 7002,
            DeviceConfig   = 7003,
            DataCoollect   = 7004,
            ConstructCalc  = 7005,
            Asutp          = 7006,
            Astra          = 7007,
            CustomSource   = 7008,
            TypicalExch    = 7009,
            RestServices   = 7010,
            ExchangeLog    = 7011,
            AsduMapping    = 7012,
            AsduNsiDataImport     = 7013,
            AsduMatching   = 7014,
            AsspootiMapping = 7015,
            AsduNsiMetadata = 7016,
            AsduNsiData = 7017,            
        // отчеты
        Report = 8000,
            SapBo          = 8001,
        // пользователь
        User               = 9000,
    }
    public abstract class PermissionNode : PropertyChangedBase
    {
        public LinkType LinkType { get; protected set; }
        public LinkType ParentLinkType { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
    }
    /// <summary> ссылка на функционал </summary>
    public class Leaf : PermissionNode
    {
        public Leaf(LinkType linkType)
        {
            NodeType = NodeType.Leaf;
            LinkType = linkType;
        }
    }
    /// <summary> подраздел, хранит ссылки </summary>
    public class Branch : PermissionNode
    {
        public Branch(LinkType linkType)
        {
            LinkType = linkType;
            NodeType = NodeType.Branch;
            Childs = new ObservableCollection<PermissionNode>();
        }
        private ObservableCollection<PermissionNode> _childs;
        public ObservableCollection<PermissionNode> Childs
        {
            get { return _childs; }
            set
            {
                _childs = value;
                OnPropertyChanged(() => Childs);
            }
        }
    }
    public class PermissionDTO2
    {
        public PermissionDTO2()
        {
            Image = string.Empty;
        }

        public string Name { get; set; }
        public string Image { get; set; }
        public string Uri { get; set; }
    }
    /// <summary> А  В  Т  О  Р  И  З  А  Ц  И  Я       2
    /// 
    /// 0. администратору ИУС ПТП - доступны все привилегии
    /// 1. у пользователя могут быть несколько ролей
    /// 2. пользователю назначается наивысшая привилегия из имеющихся в списке ролей пользователя
    /// 3. пользователь без ролей по дефолту не имеет доступ к функционалу?
    /// 4. 
    /// 
    /// seq: 
    /// 2. подгрузить пермишины для ролей
    /// 3. 
    /// 
    /// </summary>
    public class Authorization2
    {
#region constructor
        public static Authorization2 Inst { get; } = new Authorization2();
        public Authorization2()
        {
            Menu = new Branch(LinkType.Root) { Name = RootName };
        }
#endregion
#region variables
        private const string RootName = "root";
        private List<RoleDTO> _userRoles;
        private IEnumerable<PermissionDTO> _permissions;
        private Dictionary<int, int> _userMaxPerm;
        //
        public Branch Menu { get; }
#endregion
#region methods
        public bool IsAuthorized(LinkType linkType)
        {
            if (IsAdminRole()) return true;
            //
            var link = (int)linkType;
            if (!_userMaxPerm.ContainsKey(link)) return false;
            //
            return _userMaxPerm[link] > 0;
        }
        public bool IsEditable(LinkType linkType)
        {
            if (IsAdminRole()) return true;
            // 
            if (!_userMaxPerm.ContainsKey((int) linkType)) return false;
            //
            return _userMaxPerm[(int)linkType] == 2;
        }
        public void Init(List<RoleDTO> userRoles, IEnumerable<PermissionDTO> permissions)
        {
            _userRoles   = userRoles;
            _permissions = permissions;
            // todo: if (null userRoles permissions) {}
            var join = userRoles.Join(_permissions, 
                                        role => role.Id,
                                        permission => permission.RoleId,
                                        (r, pe) => new {
                                            RoleId = r.Id,
                                            LinkId = pe.ItemId,
                                            Perm   = pe.Permission
                                        });
            _userMaxPerm = join.ToLookup(k => k.LinkId)
                               .ToDictionary(k => k.Key, v => v.Max(e => e.Perm));
        }
        public Branch AddBranch(LinkType linkType, string name)
        {
            var parent = new Branch(linkType) { Name = name };
            Menu.Childs.Add(parent);            
            return parent;
        }
        public void AddLeaf(Branch branch, LinkType linkType, string name)
        {
            var link = new Leaf(linkType) { Name = name };
            branch.Childs.Add(link);
            link.ParentLinkType = branch.LinkType;
        }
        #endregion
        /// <summary>
        /// 
        /// Description -> Администраторы ИУС ПТП
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAdminRole()
        {
            return _userRoles.Any(e => e.Description.Contains("Администратор") &&
                                       e.Description.Contains("ИУС") &&
                                       e.Description.Contains("ПТП"));
        }
        public bool IsReportAdminRole()
        {
            return _userRoles.Any(e => 
                e.Name.Equals("reportadmin", StringComparison.InvariantCultureIgnoreCase));            
        }

//        private async void UpdateUserRoles()
//        {
//            var userRoles = await new UserManagementServiceProxy()
//                .GetUserRolesAsync(UserProfile.Current.Id).ConfigureAwait(true);
//        }
    }
}
#region trash

//IsAdminRole = GetValueAdminRole();
//IsReportAdmin = GetValueReportAdminRole();

//public bool IsAdminRole { get; private set; }
//public bool IsReportAdmin { get; private set; }
#endregion
