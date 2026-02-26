using System;
using System.Collections.Generic;
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
    [AddinAttribute(Caption = "VectorDB", BranchType = EnumPointType.DataPoint, Name = "VectorDBNode.Beep", misc = "Beep", iconimage = "database.png", menu = "Beep", ObjectType = "Beep")]
    public class VectorDBNode : IBranch 
    {
        public VectorDBNode() { }

        public VectorDBNode(ConnectionProperties i, ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            IconImageName = pimagename;
            DataSourceName = pBranchText;
            _conn = i;
            DataSourceConnectionGuidID = i.GuidID;
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
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        private ConnectionProperties _conn;
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.DataPoint;
        public int BranchID { get; set; }
        public string IconImageName { get; set; }
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VectorDB";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public List<Delegate> Delegates { get; set; }
        public int ID { get; set; }
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int MiscID { get; set; }
        public string ObjectType { get; set; } = "Beep";

        public IErrorsInfo CreateChildNodes()
        {
            return GetDatabaseEntites();
        }

        [CommandAttribute(Caption = "Get Entities", iconimage = "getchilds.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep")]
        public IErrorsInfo GetDatabaseEntites()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            PassedArgs passedArgs = new PassedArgs { DatasourceName = BranchText };
            try
            {
                DataSourceDefaultMethods.GetEntities(this, DMEEditor, Visutil);
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Connecting to DataSource ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
                passedArgs.Messege = "Could not Open Connection";
                Visutil.PasstoWaitForm(passedArgs);
                Visutil.CloseWaitForm();
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh Entities", iconimage = "refresh.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep")]
        public IErrorsInfo RefreshDatabaseEntites()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            PassedArgs passedArgs = new PassedArgs { DatasourceName = BranchText };
            try
            {
                DataSourceDefaultMethods.RefreshEntities(this, DMEEditor, Visutil);
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Connecting to DataSource ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
            }
            return DMEEditor.ErrorObject;
        }

        public IBranch CreateCategoryNode(CategoryFolder p) { throw new NotImplementedException(); }
        public IErrorsInfo ExecuteBranchAction(string ActionName) { throw new NotImplementedException(); }
        public IErrorsInfo MenuItemClicked(string ActionNam) { throw new NotImplementedException(); }
        public IErrorsInfo RemoveChildNodes() { throw new NotImplementedException(); }
    }
}

