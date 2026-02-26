using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Addin;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.DriversConfigurations;

namespace TheTechIdea.Beep.TreeNodes.VectorDB
{
    [AddinAttribute(Caption = "VectorDB", BranchType = EnumPointType.Root, Name = "VectorDBRootNode.Beep", misc = "Beep", iconimage = "vectordb.svg", menu = "DataSource", ObjectType = "Beep", Category = DatasourceCategory.VectorDB)]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "DATASOURCEROOT", RootNodeName = "DataSourcesRootNode")]
    public class VectorDBRootNode : IBranch  
    {
        public VectorDBRootNode()
        {
            IsDataSourceNode = true;
        }

        public VectorDBRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
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
                BranchID = ID;
            }
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
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
        public string Name { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string BranchText { get; set; } = "VectorDB";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; } = 0;
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "database.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VectorDB";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public List<Delegate> Delegates { get; set; }
        public int ID { get; set; }
        public int Order { get; set; } = 3;
        public int MiscID { get; set; }
        public IAppManager Visutil { get; set; }
        public object TreeStrucure { get; set; }
        public string ObjectType { get; set; } = "Beep";

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                foreach (ConnectionProperties i in DMEEditor.ConfigEditor.DataConnections.Where(c => c.Category == DatasourceCategory.VectorDB && c.IsComposite == false))
                {
                    if (TreeEditor.Treebranchhandler.CheckifBranchExistinCategory(i.ConnectionName, "VectorDB") == null)
                    {
                        if (!ChildBranchs.Any(p => p.GuidID.Equals(i.GuidID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            CreateDBNode(i);
                            i.Drawn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Create Child Nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo CreateDBNode(ConnectionProperties i)
        {
            try
            {
                DriversConfigurations.ConnectionDriversConfig drv = DMEEditor.ConfigEditor.DataDriversClasses.Where(p => p.PackageName == i.DriverName).FirstOrDefault();
                string icon = drv is null ? "unknowndatasource.svg" : drv.iconname;
                VectorDBNode database = new VectorDBNode(i, TreeEditor, DMEEditor, this, i.ConnectionName, TreeEditor.SeqID, EnumPointType.DataPoint, icon);
                database.DataSource = DataSource;
                database.DataSourceName = i.ConnectionName;
                database.DataSourceConnectionGuidID = i.GuidID;
                database.GuidID = i.GuidID;
                database.IconImageName = icon;

                TreeEditor.Treebranchhandler.AddBranch(this, database);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            // Implementation for Category node can be added later as needed
            throw new NotImplementedException();
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName) { throw new NotImplementedException(); }
        public IErrorsInfo MenuItemClicked(string ActionNam) { throw new NotImplementedException(); }
        public IErrorsInfo RemoveChildNodes() { throw new NotImplementedException(); }
    }
}

