using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using TheTechIdea.Beep;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;



namespace TheTechIdea.Beep.TreeNodes.RDBMS
{
    [AddinAttribute(Caption = "RDBMS", BranchType = EnumPointType.DataPoint, Name = "DatabaseNode.Beep", misc = "Beep", iconimage = "database.png", menu = "Beep", ObjectType = "Beep")]
    public class DatabaseNode  : IBranch 
    {
        public DatabaseNode()
        {

        }
        public DatabaseNode(ConnectionProperties i,ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
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
                ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {
                    ID = pID;
                    BranchID = ID;
                }
                DMEEditor.AddLogMessage("Success", "Set Config OK", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
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
         public IBranch ParentBranch { get  ; set  ; }
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
        public string BranchClass { get; set; } = "RDBMS";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public List<Delegate> Delegates { get; set; }
        public int ID { get; set; }
        public object TreeStrucure { get ; set ; }
        public  IAppManager  Visutil  { get ; set ; }
        public int MiscID { get; set; }
        public string ObjectType { get; set; } = "Beep";
        public IErrorsInfo CreateChildNodes()
        {
           return GetDatabaseEntites();
        }
        public IErrorsInfo CreateDelegateMenu()
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
        [CommandAttribute(Caption = "Edit DB Connection", Name = "EditDBConnection", iconimage = "dbconnection.png")]
        public IErrorsInfo EditDBConnection()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //   DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                string[] args = { "New Web API ", null, null };
                List<ObjectItem> ob = new List<ObjectItem>(); ;
                ObjectItem it = new ObjectItem();
                it.obj = this;
                it.Name = "Branch";
                ob.Add(it);


                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = null,
                    CurrentEntity = BranchText,
                    Id = 0,
                    ObjectType = "DB",
                    DataSource = null,
                    ObjectName = BranchText,
                    ParameterString1 = DataSourceConnectionGuidID,
                    Objects = ob,

                    DatasourceName = BranchText,
                    EventType = "EDITDB"

                };
                // ActionNeeded?.Invoke(this, Passedarguments);
                Visutil.ShowPage("uc_Database", Passedarguments);



            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Database Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Caption = "Get Entities", iconimage = "getchilds.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep")]
        public IErrorsInfo GetDatabaseEntites()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //     DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
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
       
        [CommandAttribute(Caption = "Refresh Entities", iconimage = "refresh.png", PointType = EnumPointType.DataPoint, ObjectType ="Beep")]
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
                DMEEditor.ErrorObject.Ex = ex;
                passedArgs.Messege = "Could not Open Connection";
                Visutil.PasstoWaitForm(passedArgs);
                Visutil.CloseWaitForm();
            }
            return DMEEditor.ErrorObject;

        }
        //[CommandAttribute(Caption = "Drop Entities", iconimage = "remove.png")]
        //public IErrorsInfo DropEntities()
        //{
        //    DMEEditor.ErrorObject.Flag = Errors.Ok;
        //    try
        //    {
        //        if (Visutil.DialogManager.InputBoxYesNo("Beep DM", "Are you sure you ?") ==Beep.Vis.Module.BeepDialogResult.Yes)
        //        {
        //            if (TreeEditor.SelectedBranchs.Count > 0)
        //            {
        //                foreach (int item in TreeEditor.SelectedBranchs)
        //                {
        //                    IBranch br = TreeEditor.Treebranchhandler.GetBranch(item);
        //                    if (br != null)
        //                    {
        //                        if (br.DataSourceName == DataSourceName)
        //                        {
        //                            IDataSource srcds = DMEEditor.GetDataSource(br.DataSourceName);
        //                            EntityStructure ent = DataSource.GetEntityStructure(br.BranchText, false);
        //                            DataSource.ExecuteSql($"Drop Table {ent.DatasourceEntityName}");
        //                            if (DMEEditor.ErrorObject.Flag == Errors.Ok)
        //                            {
        //                                TreeEditor.Treebranchhandler.RemoveBranch(br);
        //                                DataSource.Entities.RemoveAt(DataSource.Entities.FindIndex(p => p.DatasourceEntityName == ent.DatasourceEntityName));
        //                                DMEEditor.AddLogMessage("Success", $"Droped Entity {ent.EntityName}", DateTime.Now, -1, null, Errors.Ok);
        //                            }
        //                            else
        //                            {
        //                                DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {ent.EntityName} - {DMEEditor.ErrorObject.Message}", DateTime.Now, -1, null, Errors.Failed);
        //                            }
        //                        }
        //                    }
                           
        //                }
        //                DMEEditor.ConfigEditor.SaveDataSourceEntitiesValues(new TheTechIdea.Beep.ConfigUtil.DatasourceEntities { datasourcename = DataSourceName, Entities = DataSource.Entities });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.ErrorObject.Flag = Errors.Failed;
        //        DMEEditor.ErrorObject.Ex = ex;
        //        DMEEditor.AddLogMessage("Fail", $"Error Drpping Entity {EntityStructure.EntityName} - {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
        //    }
        //    return DMEEditor.ErrorObject;
        //}
        //[CommandAttribute(Caption = "Create POCO Classes")]
        //public IErrorsInfo CreatePOCOlasses()
        //{
        //    DMEEditor.ErrorObject.Flag = Errors.Ok;
        //    PassedArgs passedArgs = new PassedArgs { DatasourceName = BranchText };
        //    try
        //    {
        //        string iconimage;
        //        DataSource = (IRDBSource)DMEEditor.GetDataSource(BranchText);
        //        if (DataSource != null)
        //        {
        //            Visutil.ShowWaitForm(passedArgs);
        //            DataSource.Openconnection();

        //            if (DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
        //            {
        //                if (Visutil.DialogManager.InputBoxYesNo("Beep DM", "Are you sure, this might take some time?") == Beep.Vis.Module.BeepDialogResult.Yes)
        //                {
                           
        //                    int i = 0;
        //                    passedArgs.Messege = $"Creating POCO {DataSource.EntitiesNames.Count} Entities";
        //                    Visutil.PasstoWaitForm(passedArgs);
        //                    foreach (string tb in DataSource.EntitiesNames)
        //                    {
        //                   //     TreeEditor.AddCommentsWaiting($"{i} - Added {tb} to {DataSourceName}");
        //                        EntityStructure ent = DataSource.GetEntityStructure(tb, true);
                              
        //                        DMEEditor.classCreator.CreateClass(ent.EntityName, ent.Fields, DMEEditor.ConfigEditor.ExePath);
        //                        i += 1;
        //                    }
        //                    passedArgs.Messege = "Done";
        //                    Visutil.PasstoWaitForm(passedArgs);
        //                }

        //            }
        //            else
        //            {
        //                passedArgs.Messege = "Could not Open Connection";
        //                Visutil.PasstoWaitForm(passedArgs);
        //            }


        //        }

        //        Visutil.CloseWaitForm();

        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.Logger.WriteLog($"Error in Creating POCO Entites ({ex.Message}) ");
        //        DMEEditor.ErrorObject.Flag = Errors.Failed;
        //        DMEEditor.ErrorObject.Ex = ex;
        //        passedArgs.Messege = "Could not Open Connection";
        //        Visutil.PasstoWaitForm(passedArgs);
        //        Visutil.CloseWaitForm();
        //    }
        //    return DMEEditor.ErrorObject;

        //}
        private void update()
        {

        }

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }
    }
}
