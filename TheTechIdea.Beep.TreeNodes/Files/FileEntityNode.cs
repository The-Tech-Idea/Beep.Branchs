using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.TreeNodes.Files
{
    [AddinAttribute(Caption = "Files", BranchType = EnumPointType.DataPoint, Name = "FileEntityNode.Beep", misc = "Beep", iconimage = "file.png", menu = "Beep", ObjectType = "Beep")]
    public class FileEntityNode : IBranch 
    {

        public FileEntityNode()
        {


        }
        public FileEntityNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string DataSourceGuidID)
        {



            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            DataSourceName = pBranchText;
            string ext = Path.GetExtension(BranchText).Remove(0, 1);
            IconImageName = ext + ".png";
            DataSourceConnectionGuidID = DataSourceGuidID;
            cn = new ConnectionProperties();
            cn = DMEEditor.ConfigEditor.DataConnections.FirstOrDefault(p => p.GuidID == DataSourceGuidID);
            if (cn != null)
            {
                DataSourceName = cn.ConnectionName;
            }

            if (pID != 0)

            {
                ID = pID;
                BranchID = pID;
            }
        }
        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;

        #region "Properties"
        public bool IsDataSourceNode { get; set; } = true;
        ConnectionProperties cn=new ConnectionProperties();
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "Files";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.DataPoint;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "file.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "FILE";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IAppManager  Visutil { get; set; }
        public int MiscID { get; set; }

     
       // public event EventHandler<PassedArgs> BranchSelected;
       // public event EventHandler<PassedArgs> BranchDragEnter;
       // public event EventHandler<PassedArgs> BranchDragDrop;
       // public event EventHandler<PassedArgs> BranchDragLeave;
       // public event EventHandler<PassedArgs> BranchDragClick;
       // public event EventHandler<PassedArgs> BranchDragDoubleClick;
       // public event EventHandler<PassedArgs> ActionNeeded;
        #endregion "Properties"
        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                CreateNodes();

                DMEEditor.AddLogMessage("Success", "Added Child Nodes", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Child Nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

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
      ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {
                    ID = pID;
                }

                //   DMEEditor.AddLogMessage("Success", "Set Config OK", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        #endregion "Interface Methods"
        #region "Exposed Interface"
        [CommandAttribute(Caption = "Get Sheets", Hidden = false, iconimage = "getchilds.png")]
        public IErrorsInfo GetSheets()
        {

            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                CreateChildNodes();
               // DMEEditor.AddLogMessage("Success", "Config File", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Get Sheets";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Refresh Sheets", Hidden = false, iconimage = "refresh.png")]
        public IErrorsInfo RefreshSheets()
        {
            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                int i = 0;
                DataSource = (IDataSource)DMEEditor.GetDataSource(BranchText);
                if (DataSource != null)
                {
                    
                    DataSource.GetEntitesList();
                    if (DataSource.Entities.Count > 0)
                    {
                        DataSource.EntitiesNames = DataSource.Entities.Select(o => o.EntityName).ToList();
                        if (DataSource.EntitiesNames.Count > 0)
                        {
                            foreach (string n in DataSource.EntitiesNames)
                            {
                                EntityStructure entity = DataSource.GetEntityStructure(n, true);
                                CreateFileItemSheetsNode(i, n);
                                i += 1;
                            }

                        }
                    }
                }
                DMEEditor.AddLogMessage("Success", "Created child Nodes", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Get Sheets";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Edit File Connection", Name = "EditFileConnection", iconimage = "EditFileConnection.png")]
        public IErrorsInfo EditFileConnection()
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
                    ObjectType = "FILE",
                    DataSource = null,
                    ObjectName = BranchText,
                    ParameterString1 = DataSourceConnectionGuidID,
                    Objects = ob,

                    DatasourceName = BranchText,
                    EventType = "EDITFILE"

                };
                // ActionNeeded?.Invoke(this, Passedarguments);
                Visutil.ShowPage("uc_File", Passedarguments);



            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Database Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }
        //[BranchDelegate(Caption = "Remove", Hidden = false)]
        //public IErrorsInfo Remove()
        //{

        //    try
        //    {


        //        DMEEditor.AddLogMessage("Success", "Remove File", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Remove File";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        //[CommandAttribute(Caption = "Remove", iconimage = "remove.png")]
        //public IErrorsInfo Remove()
        //{

        //    try
        //    {
        //        if (Visutil.Controlmanager.InputBoxYesNo("Remove", "Area you Sure ? you want to remove File???") == Beep.Vis.Module.BeepDialogResult.Yes)
        //        {

        //            try
        //            {
        //               // DMEEditor.viewEditor.Views.Remove(DMEEditor.viewEditor.Views.Where(x => x.ViewName == DataView.ViewName).FirstOrDefault());
        //                DMEEditor.ConfigEditor.RemoveDataConnection(BranchText);
        //                DMEEditor.RemoveDataDource(BranchText);
        //                TreeEditor.Treebranchhandler.RemoveBranch(this);
        //                TreeEditor.Treebranchhandler.RemoveEntityFromCategory("FILE",TreeEditor.Treebranchhandler.GetBranch(ParentBranchID).BranchText, BranchText);
        //                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
        //                DMEEditor.AddLogMessage("Success", "Removed View from Views List", DateTime.Now, 0, null, Errors.Ok);
        //            }
        //            catch (Exception ex)
        //            {
        //                string mes = "Could not Remove View from Views List";
        //                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //            };


        //            TreeEditor.Treebranchhandler.RemoveBranch(this);
        //        }


        //        DMEEditor.AddLogMessage("Success", "Remove View", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Remove View";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        //[CommandAttribute(Caption = "Copy Entities")]
        //public IErrorsInfo CopyEntities()
        //{

        //    try
        //    {
        //        List<string> ents = new List<string>();
        //        if (TreeEditor.SelectedBranchs.Count > 0)
        //        {
        //            if (DataSource == null)
        //            {
        //                DataSource = DMEEditor.GetDataSource(DataSourceName);
        //            }
        //            if (DataSource != null)
        //            {
        //                foreach (int item in TreeEditor.SelectedBranchs)
        //                {
        //                    IBranch br = TreeEditor.Treebranchhandler.GetBranch(item);
        //                    ents.Add(br.BranchText);
        //                    // EntityStructure = DataSource.GetEntityStructure(br.BranchText, true);

        //                }
        //                IBranch pbr = TreeEditor.Treebranchhandler.GetBranch(ParentBranchID);
        //                List<ObjectItem> ob = new List<ObjectItem>(); ;
        //                ObjectItem it = new ObjectItem();
        //                it.obj = pbr;
        //                it.Name = "ParentBranch";
        //                ob.Add(it);

        //                PassedArgs args = new PassedArgs
        //                {
        //                    ObjectName = "DATABASE",
        //                    ObjectType = "TABLE",
        //                    EventType = "COPYENTITIES",
        //                    ParameterString1 = "COPYENTITIES",
        //                    DataSource = DataSource,
        //                    DatasourceName = DataSource.DatasourceName,
        //                    CurrentEntity = BranchText,
        //                    EntitiesNames = ents,
        //                    Objects = ob
        //                };

        //                DMEEditor.Passedarguments = args;
        //            }
        //            else
        //            {
        //                DMEEditor.AddLogMessage("Fail", "Could not get DataSource", DateTime.Now, -1, null, Errors.Failed);
        //            }

        //        }

        //        // TreeEditor.SendActionFromBranchToBranch(pbr, this, "Create View using Table");

        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Copy Entites";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        #endregion Exposed Interface"
        #region "Other Methods"
        public IErrorsInfo CreateNodes()
        {
          //  TxtXlsCSVFileSource fs = null; ;
            try
            {
                int i = 0;
                DataSource = (IDataSource)DMEEditor.GetDataSource(BranchText);
                
                if (DataSource != null)
                {
                    DataSource.Openconnection();
                    if(DataSource.ConnectionStatus== System.Data.ConnectionState.Open)
                    {
                        DataSource.GetEntitesList();


                        if (DataSource.Entities.Count > 0)
                        {
                            DataSource.EntitiesNames = DataSource.Entities.Select(o => o.EntityName).ToList();
                            if (DataSource.EntitiesNames.Count > 0)
                            {
                                foreach (string n in DataSource.EntitiesNames)
                                {
                                    CreateFileItemSheetsNode(i, n);
                                    i += 1;
                                }

                            }
                        }
                    }else
                    {
                        string mes = "Error : Could Not Find File";
                        DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                        Visutil.Controlmanager.MsgBox("Beep", mes);
                    }


                }
                else
                {
                    string mes = "Error : Could Not Find File DataSource";
                    DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                    Visutil.Controlmanager.MsgBox("Beep", mes);
                }

                DMEEditor.AddLogMessage("Success", "Created child Nodes", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Create child Nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }
        private IErrorsInfo CreateFileItemSheetsNode(int id, string Sheetname)
        {

            try
            {
                FileEntitySheetNode fileitemsheet = new FileEntitySheetNode(TreeEditor, DMEEditor, this, Sheetname, TreeEditor.SeqID, EnumPointType.Entity, IconImageName, BranchText);
                fileitemsheet.DataSource = DataSource;
                fileitemsheet.DataSourceName = DataSourceName;

               // TreeEditor.AddBranchToParentInBranchsOnly(this,fileitemsheet);
                TreeEditor.Treebranchhandler.AddBranch(this,fileitemsheet);

                DMEEditor.AddLogMessage("Success", "Added sheet", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add sheet";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
          
           
        }

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }
        #endregion"Other Methods"

    }
}
