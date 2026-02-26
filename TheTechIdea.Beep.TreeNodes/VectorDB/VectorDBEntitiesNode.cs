using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.TreeNodes.VectorDB
{
    [AddinAttribute(Caption = "VectorDB", BranchType = EnumPointType.Entity, Name = "VectorDBEntitiesNode.Beep", misc = "Beep", iconimage = "database.png", menu = "Beep", ObjectType = "Beep")]
    public class VectorDBEntitiesNode : IBranch 
    {
        public VectorDBEntitiesNode() { }

        public VectorDBEntitiesNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, IDataSource ds)
        {
            DataSource = ds;
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = EnumPointType.Entity;
            IconImageName = pimagename;
            EntityStructure = new EntityStructure();
            EntityStructure.DataSourceID = ds.DatasourceName;
            EntityStructure.Viewtype = ViewType.Table;
            EntityStructure.EntityName = pBranchText;
            DataSourceName = ds.DatasourceName;

            if (pID != 0)
            {
                ID = pID;
                BranchID = ID;
            }
        }

        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;
        public bool IsDataSourceNode { get; set; } = true;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public IBranch ParentBranch { get; set; }
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = "";
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; }
        public string IconImageName { get; set; }
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VectorDB";
        public List<IBranch> ChildBranchs { get; set; }
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public string ObjectType { get; set; } = "Beep";
        public int MiscID { get; set; }
       
        public IErrorsInfo CreateChildNodes()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo MenuItemClicked(string ActionNam)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo RemoveChildNodes()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {
                    ID = pID;
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }
    }
}