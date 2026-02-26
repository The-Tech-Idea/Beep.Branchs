using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DriversConfigurations;

namespace TheTechIdea.Beep.TreeNodes.MLModel
{
    [AddinAttribute(Caption = "MLModel", BranchType = EnumPointType.Category, Name = "MLModelCategoryNode.Beep", misc = "Beep", iconimage = "category.png", menu = "Beep", ObjectType = "Beep")]
    public class MLModelCategoryNode : IBranch 
    {
        public MLModelCategoryNode() { }

        public MLModelCategoryNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
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
        public int ID { get; set; }
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Category;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "category.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "MLModel";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public List<Delegate> Delegates { get; set; }
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int MiscID { get; set; }
        public string ObjectType { get; set; } = "Beep";

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                foreach (CategoryFolder p in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName.Equals("MLModel", StringComparison.InvariantCultureIgnoreCase) && x.FolderName == BranchText))
                {
                    foreach (string item in p.items)
                    {
                        ConnectionProperties i = DMEEditor.ConfigEditor.DataConnections.Where(x => x.ConnectionName == item).FirstOrDefault();
                        if (i != null)
                        {
                            CreateDBNode(i);
                            i.Drawn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo CreateDBNode(ConnectionProperties i)
        {
            try
            {
                ConnectionDriversConfig drv = DMEEditor.ConfigEditor.DataDriversClasses.Where(p => p.PackageName == i.DriverName).FirstOrDefault();
                string icon = drv is null ? "unknowndatasource.svg" : drv.iconname;
                MLModelNode database = new MLModelNode(i, TreeEditor, DMEEditor, this, i.ConnectionName, TreeEditor.SeqID, EnumPointType.DataPoint, icon);
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
            throw new NotImplementedException();
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName) { throw new NotImplementedException(); }
        public IErrorsInfo MenuItemClicked(string ActionNam) { throw new NotImplementedException(); }
        public IErrorsInfo RemoveChildNodes() { throw new NotImplementedException(); }
    }
}