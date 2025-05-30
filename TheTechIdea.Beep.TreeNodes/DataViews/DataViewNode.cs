﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.DataView;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    [AddinAttribute(Caption = "DataView", BranchType = EnumPointType.DataPoint, Name = "DataViewNode.Beep", misc = "Beep", iconimage = "dataview.png", menu = "Beep", ObjectType = "Beep")]
    public class DataViewNode  : IBranch 
    {
        public DataViewNode()
        {

        }
        public DataViewNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename,string ConnectionName)
        {

            DataSourceName = ConnectionName;
            ID = pID;
            BranchID = pID;

            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
        //   IconImageName = pimagename;

            ds = (IDataViewDataSource)DMEEditor.GetDataSource(ConnectionName);
            //if (ds.Entities.Count > 0)
            //{
            //    ds.Entities.Clear();
            //    DMEEditor.ConfigEditor.SaveDataconnectionsValues();
            //}
            if (ds != null) MiscID = ds.ViewID ; 
          
       
        }
        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;
        #region "Properties"
        public bool IsDataSourceNode { get; set; } = true;
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
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.DataPoint;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "dataviewnode.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VIEW";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IAppManager  Visutil { get; set; }
        public int MiscID { get; set; }
        IDataViewDataSource ds;
        public IDMDataView DataView
        {
            get
            {
                if (ds != null)
                {
                    return ds.DataView;
                }
                else
                    return null;
               
            }
            set
            {
                ds.DataView = value;
            }
        }
     
        #endregion "Properties"
        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                CreateDataViewMethod();
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
                ID = pID;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {
                    BranchID = pID;
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
        [CommandAttribute(Caption = "Edit", iconimage = "edit_entity.png", ObjectType = "Beep")]
        public IErrorsInfo Edit()
        {

            try
            {

                ds = (IDataViewDataSource)DMEEditor.GetDataSource(DataSourceName);
               
                if (ds != null)
                {
                    ds.Openconnection();
                    if (ds.ConnectionStatus != System.Data.ConnectionState.Open)
                    {
                        DMEEditor.Logger.WriteLog($"Could not Find DataView File " + DataSourceName);
                    }
                    else
                    {
                        string[] args = { "New Query Entity", null, null };
                        List<ObjectItem> ob = new List<ObjectItem>(); ;
                        ObjectItem it = new ObjectItem();
                        it.obj = this;
                        it.Name = "Branch";
                        ob.Add(it);
                        IBranch RootCompositeLayerBranch = TreeEditor.Branches[TreeEditor.Branches.FindIndex(x => x.BranchClass == "VIEW" && x.BranchType == EnumPointType.Root)];
                        it = new ObjectItem();
                        it.obj = RootCompositeLayerBranch;
                        it.Name = "RootAppBranch";
                        ob.Add(it);
                        PassedArgs Passedarguments = new PassedArgs
                        {
                            Addin = null,
                            AddinName = null,
                            AddinType = "",
                            DMView = DataView,
                            CurrentEntity = BranchText,
                            Id = 0,
                            ObjectType = "EDITVIEW",
                           // DataSource = ds,
                            ObjectName = DataView.ViewName,

                            Objects = ob,

                            DatasourceName = null,
                            EventType = "EDITVIEW"

                        };
                        Passedarguments.Objects.Add(new ObjectItem() { Name = "TitleText", obj = $"Edit {DataView.ViewName}" });
                        Visutil.ShowPage("uc_ViewEditor", Passedarguments);
                    }
                }
                else
                {
                    DMEEditor.Logger.WriteLog($"Could not Find DataView File " + DataSourceName);
                }



            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling DataView Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Get Entities", iconimage = "getchilds.png", ObjectType = "Beep")]
        public IErrorsInfo CreateViewEntites()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            DMEEditor.Logger.WriteLog($"Filling View Entites ) ");
          //  string iconname;
           // CreateDataViewMethod();
            try
            {

                ds = (IDataViewDataSource)DMEEditor.GetDataSource(DataSourceName);
                ds.Openconnection();

                if (ds != null)
                {
                    if (ds.ConnectionStatus != System.Data.ConnectionState.Open)
                    {
                        DMEEditor.Logger.WriteLog($"Could not Find DataView File " + DataSourceName);
                    }
                    else
                    {
                        bool loadv = false;
                        if (ChildBranchs.Count > 0)
                        {
                            if (Visutil.DialogManager.InputBoxYesNo("Beep", "Do you want to over write th existing View Structure?") == BeepDialogResult.Yes)
                            {
                                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                                ds.LoadView();
                                loadv = true;
                            }
                        }
                        else
                        {
                            if (DataView.Entities != null)
                            {
                                if (DataView.Entities.Count == 0)
                                {
                                    ds.LoadView();
                                }
                            }

                            loadv = true;
                        }
                        if (loadv)
                        {
                            if (ds != null)
                            {
                                if (DataView != null)
                                {
                                    List<EntityStructure> cr = DataView.Entities.Where(cx => cx.ParentId == 0).ToList();
                                    int i = 0;
                                    foreach (EntityStructure tb in cr)
                                    {

                                        DataViewEntitiesNode dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, this, tb.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(tb.Viewtype), DataView.DataViewDataSourceID, tb);
                                        if (string.IsNullOrEmpty(tb.DatasourceEntityName))
                                        {
                                            DataView.Entities[ds.EntityListIndex(tb.EntityName)].DatasourceEntityName = tb.EntityName;
                                        }
                                        dbent.ID = tb.Id;
                                        TreeEditor.Treebranchhandler.AddBranch(this, dbent);
                                        dbent.CreateChildNodes();
                                        TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                                        i += 1;

                                    }
                                    ds.WriteDataViewFile(DataSourceName);
                                    DMEEditor.ConfigEditor.SaveDataSourceEntitiesValues(new DatasourceEntities { datasourcename = DataSourceName, Entities = DataView.Entities });
                                }
                            }
                            else
                            {
                                string mes = "Error : Could Not Find DataView File";
                                DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                                Visutil.DialogManager.MsgBox("Beep", mes);
                               
                            }
                            SaveView();
                        }
                    }
                }
                else
                {
                    string mes = "Error : Could Not Find DataView File";
                    DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                    Visutil.DialogManager.MsgBox("Beep", mes);
                }
            }
            catch (Exception ex)
            {
                
                string mes = "Error :  Filling DataView Entites ({ex.Message})";
                DMEEditor.AddLogMessage("Beep", mes, DateTime.Now, -1, mes, Errors.Failed);
                Visutil.DialogManager.MsgBox("Beep", mes);
            }
            return DMEEditor.ErrorObject;

        }
        
        [CommandAttribute(Caption = "Save", iconimage = "save.png")]
        public IErrorsInfo SaveView()
        {

            try
            {
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
               // ds.Dataview=DataView;
                ds.WriteDataViewFile(DataSourceName);
           
                DMEEditor.AddLogMessage("Success", "Saved View", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Save View";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Caption = "Remove", iconimage = "remove.png")]
        public IErrorsInfo RemoveView()
        {
            string file=string.Empty;
            try
            {
                if (Visutil.DialogManager.InputBoxYesNo("Remove View", "Area you Sure ? you want to remove View???") == BeepDialogResult.Yes)
                {
                    ConnectionProperties cn = DMEEditor.ConfigEditor.DataConnections.Where(x => x.ConnectionName.Equals(Path.GetFileName(DataSourceName),StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if(cn==null)
                    {
                        cn = DMEEditor.ConfigEditor.DataConnections.Where(x => x.GuidID.Equals(DataView.GuidID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    }
                    if(cn != null)
                    {
                        if(cn.FilePath != null && cn.FileName != null)
                        {
                            file = Path.Combine(cn.FilePath, cn.FileName);
                        }
                        
                        try
                        {

                            //ds.RemoveDataViewByVID(DataView.VID);
                            DMEEditor.ConfigEditor.RemoveDataConnection(DataSourceName);
                            DMEEditor.RemoveDataDource(DataSourceName);

                            DMEEditor.AddLogMessage("Success", "Removed View from Views List", DateTime.Now, 0, null, Errors.Ok);
                        }
                        catch (Exception ex)
                        {
                            string mes = "Could not Remove View from Views List";
                            DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
                        };

                        try
                        {

                            DMEEditor.DataSources.Remove(DMEEditor.DataSources.Where(x => x.DatasourceName.Equals(DataSourceName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
                            DMEEditor.AddLogMessage("Success", "Removed View from DataSource List", DateTime.Now, 0, null, Errors.Ok);
                        }
                        catch (Exception ex)
                        {
                            string mes = "Could not Removed View from DataSource List";
                            DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
                        };
                        if(string.IsNullOrEmpty(file) == false)
                        {
                            if (Visutil.DialogManager.InputBoxYesNo("Remove View", "Do you want to Delete the View File ???") == BeepDialogResult.Yes)
                            {

                                File.Delete(file);
                            }
                        }
                        
                        TreeEditor.Treebranchhandler.RemoveBranch(this);
                    }
               

                }

                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                DMEEditor.AddLogMessage("Success", "Remove View", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Remove View";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        //  [CommandAttribute(Caption = "Create Entity", iconimage = "add.png")]
        //public IErrorsInfo CreateEntity()
        //{

        //    try
        //    {
        //        string[] args = { "New Query Entity", null, null };
        //        List<ObjectItem> ob = new List<ObjectItem>(); ;
        //        ObjectItem it = new ObjectItem();
        //        it.obj = this;
        //        it.Name = "Branch";
        //        ob.Add(it);

        //        PassedArgs Passedarguments = new PassedArgs
        //        {
        //            Addin = null,
        //            AddinName = null,
        //            AddinType = "",
        //            DMView = DataView,
        //            CurrentEntity = null,
        //            Id = DataView.Entities[0].Id,
        //            ObjectType = "NEWENTITY",
        //            DataSource = ds,
        //            ObjectName = DataView.ViewName,

        //            Objects = ob,

        //            DatasourceName = ds.DatasourceName,
        //            EventType = "NEWENTITY"

        //        };
        //        //ActionNeeded?.Invoke(this, Passedarguments);
        //        Visutil.ShowPage("Uc_DataViewEntityEditor", Passedarguments);

        //        DMEEditor.AddLogMessage("Success", "Created Query Entity", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Create Query Entity";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        //[CommandAttribute(Caption = "Create Composed Layer", iconimage = "localdb.png")]
        [CommandAttribute(Caption = "Create Composed Layer", iconimage = "create.png")]
        public IErrorsInfo CreateComposedLayer()
        {

            try
            {
                string[] args = { "New Query Entity", null, null };
                List<ObjectItem> ob = new List<ObjectItem>(); ;
                ObjectItem it = new ObjectItem();
                it.obj = this;
                it.Name = "Branch";
                ob.Add(it);
                IBranch RootCompositeLayerBranch = TreeEditor.Branches[TreeEditor.Branches.FindIndex(x => x.BranchClass == "CLAYER" && x.BranchType == EnumPointType.Root)];
                it = new ObjectItem();
                it.obj = RootCompositeLayerBranch;
                it.Name = "ParentBranch";
                ob.Add(it);
                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = DataView,
                    CurrentEntity = null,
                    Id = DataView.Entities[0].Id,
                    ObjectType = "QUERYENTITY",
                 //   DataSource = ds,
                    ObjectName = DataView.ViewName,

                    Objects = ob,

                    DatasourceName = null,
                    EventType = "CLEARECOMPOSITELAYER"

                };
                Passedarguments.Objects.Add(new ObjectItem() { Name = "TitleText", obj = $"Composed Layer {DataView.ViewName}" });
                Visutil.ShowPage("uc_CreateComposedLayerFromView", Passedarguments,DisplayType.Popup);

               // DMEEditor.AddLogMessage("Success", "Created Query Entity", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Create Query Entity";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        //[CommandAttribute(Caption = "Create App", iconimage = "localdb.png"))]
        //public IErrorsInfo CreateApp()
        //{

        //    try
        //    {
        //        string[] args = { "New Query Entity", null, null };
        //        List<ObjectItem> ob = new List<ObjectItem>(); ;
        //        ObjectItem it = new ObjectItem();
        //        it.obj = this;
        //        it.Name = "Branch";
        //        ob.Add(it);
        //        IBranch RootCompositeLayerBranch = TreeEditor.Branches[TreeEditor.Branches.FindIndex(x => x.BranchClass == "APP" && x.BranchType == EnumPointType.Root)];
        //        it = new ObjectItem();
        //        it.obj = RootCompositeLayerBranch;
        //        it.Name = "RootAppBranch";
        //        ob.Add(it);
        //        PassedArgs Passedarguments = new PassedArgs
        //        {
        //            Addin = null,
        //            AddinName = null,
        //            AddinType = "",
        //            DMView = DataView,
        //            CurrentEntity = null,
        //            Id = DataView.Entities[0].Id,
        //            ObjectType = "QUERYENTITY",
        //            DataSource = ds,
        //            ObjectName = DataView.ViewName,

        //            Objects = ob,

        //            DatasourceName = null,
        //            EventType = "CREATAPP"

        //        };
        //        // ActionNeeded?.Invoke(this, Passedarguments);
        //        Visutil.ShowPage("uc_App", Passedarguments);

        //        DMEEditor.AddLogMessage("Success", "Created Query Entity", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Create Query Entity";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
        //[BranchDelegate(Caption = "Add Child Entity From IDataSource", Hidden = true)]
        //public IErrorsInfo AddChildEntityFromIDataSource()
        //{

        //    try
        //    {

        //        if (TreeEditor.args.Objects != null)
        //        {
        //            IBranch branchentity = (IBranch)TreeEditor.args.Objects.Where(x => x.Name == "ChildBranch").FirstOrDefault().obj;
        //            IDataSource childds = DMEEditor.GetDataSource(branchentity.DataSource.DatasourceName);
        //            if (childds != null)
        //            {
        //                EntityStructure entity = childds.GetEntityStructure(branchentity.BranchText, true);

        //                if (entity != null)
        //                {
        //                   // EntityStructure CurEntity = childds.GetEntityStructure(BranchText, true);
        //                    EntityStructure newentity = new EntityStructure();
        //                    newentity.Id = ds.NextHearId();
        //                    newentity.ParentId = 1;
        //                    newentity.ViewID = DataView.ViewID;
        //                    newentity.Viewtype = entity.Viewtype;
        //                    newentity.Relations = entity.Relations;
        //                    newentity.PrimaryKeys = entity.PrimaryKeys;
        //                    newentity.EntityName = entity.EntityName;
        //                    newentity.Fields = entity.Fields;
        //                    newentity.DataSourceID = entity.DataSourceID;
        //                    newentity.DatabaseType = entity.DatabaseType;
        //                    newentity.SchemaOrOwnerOrDatabase = entity.SchemaOrOwnerOrDatabase;
        //                    newentity.Created = false;
                          
        //                    ds.CreateEntityAs(newentity);

        //                    DataViewEntitiesNode dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, this, newentity.EntityName, TreeEditor.SeqID, EnumBranchType.Entity, "entity.png", DataView.DataViewDataSourceID, newentity);

        //                    TreeEditor.Treebranchhandler.AddBranch(this, dbent);
        //                    dbent.CreateChildNodes();
        //                    TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
        //                }
        //            }

        //        }



        //        DMEEditor.AddLogMessage("Success", "Created Query Entity", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        string mes = "Could not Create Query Entity";
        //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        //    };
        //    return DMEEditor.ErrorObject;
        //}
      
        [CommandAttribute(Caption = "Clear View")]
        public IErrorsInfo ClearView()
        {

            try
            {
                // IBranch CurrentBranch = TreeEditor.Branches.Where(x => x.BranchType == EnumBranchType.Root && x.BranchClass == "VIEW").FirstOrDefault();
                //
                ds = (IDataViewDataSource)DMEEditor.GetDataSource(DataSourceName);

                DMEEditor.OpenDataSource(DataSourceName);
                if (ds != null)
                {
                    DMEEditor.OpenDataSource(DataSourceName);
                  //  ds.Dataconnection.OpenConnection();
                    ds.Entities.Clear();
                    DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                    // ds.Dataview=DataView;
                    ds.WriteDataViewFile(DataSourceName);

                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Added Entity ";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        #endregion Exposed Interface"
        #region "Other Methods"
        public IErrorsInfo GetFile()
        {

            try
            {


                DMEEditor.AddLogMessage("Success", "Loaded File", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Load File";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        private IErrorsInfo CreateDataViewMethod()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
            PassedArgs passedArgs = new PassedArgs { DatasourceName = BranchText };
            try
            {
                EntityStructure ent;
                string iconimage;
                DataSource = DMEEditor.GetDataSource(BranchText);
                ds = (IDataViewDataSource)DMEEditor.GetDataSource(BranchText);
                Visutil.CloseWaitForm();
                Visutil.ShowWaitForm(passedArgs);
                if (DataSource != null)
                {

                    ds.Openconnection();
                    if (ds.ConnectionStatus == System.Data.ConnectionState.Open)
                    {
                        if (DataView.Entities != null)
                        {
                            if (DataView.Entities.Count == 0)
                            {
                                ds.LoadView();
                            }
                        }
                        passedArgs.Messege = "Connection Successful";
                        Visutil.PasstoWaitForm(passedArgs);
                        passedArgs.Messege = "Getting Entities";
                        Visutil.PasstoWaitForm(passedArgs);

                        List<string> ename = ds.Entities.Select(o => o.EntityName).ToList();
                        ds.GetEntitesList();
                        List<string> existing = ds.EntitiesNames.ToList();
                        List<string> diffnames = ename.Except(existing).ToList();
                        TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                        int i = 0;
                        if (existing.Count > 0) // there is entities in Datasource
                        {
                             foreach (string tb in diffnames) //
                            {
                                ent = DataView.Entities[ds.EntityListIndex(tb)];
                                // ent = DataSource.GetEntityStructure(tb, true);
                                if (ent != null)
                                {
                                    //if (ent.Created == false)
                                    //{
                                    //    DataSource.Entities.Remove(ent);
                                    //    DataSource.EntitiesNames.Remove(tb);
                                    //}
                                    //else
                                    //{
                                        iconimage = "databaseentities.png";
                                      
                                        DataViewEntitiesNode dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, this, ent.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(ent.Viewtype), DataView.DataViewDataSourceID, ent);
                                        dbent.DataSourceName = DataSource.DatasourceName;
                                        dbent.DataSource = DataSource;
                                        TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                                       
                                    //    TreeEditor.Treebranchhandler.AddBranch(this, dbent);
                                    dbent.CreateChildNodes();
                                    i += 1;
                                  //  }

                                }
                            }
                            passedArgs.Messege = $"Getting {existing.Count} Entities";
                            Visutil.PasstoWaitForm(passedArgs);
                            //------------------------------- Draw Existing Entities
                            foreach (string tb in existing) //
                            {
                                //ent = ds.GetEntityStructure(tb, false);
                                //if (ent.Created == false)
                                //{
                                //    ds.Entities.Remove(ent);
                                //    ds.EntitiesNames.Remove(tb);
                                //    //   iconimage = "entitynotcreated.png";
                                //}
                                //else
                                //{
                                    iconimage = "databaseentities.png";
                                    ent = DataView.Entities[ds.EntityListIndex(tb)];
                                    DataViewEntitiesNode dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, this, ent.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(ent.Viewtype), DataView.DataViewDataSourceID, ent);
                                 
                                    dbent.DataSourceName = DataSource.DatasourceName;
                                    dbent.DataSource = DataSource;
                                    TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                                   
                                    TreeEditor.Treebranchhandler.AddBranch(this, dbent);
                                dbent.CreateChildNodes();
                                i += 1;
                               // }

                            }
                            //------------------------------------------------------

                        }
                        else
                        {
                            passedArgs.Messege = passedArgs.Messege + Environment.NewLine + "No Entities Found";
                            Visutil.PasstoWaitForm(passedArgs);
                        }


                        passedArgs.Messege = passedArgs.Messege + Environment.NewLine + "Done";
                        Visutil.PasstoWaitForm(passedArgs);
                        DMEEditor.ConfigEditor.SaveDataSourceEntitiesValues(new TheTechIdea.Beep.ConfigUtil.DatasourceEntities { datasourcename = DataSourceName, Entities = DataSource.Entities });
                    }
                    else
                    {
                        passedArgs.Messege = passedArgs.Messege + Environment.NewLine + "Could not Open Connection";
                        Visutil.PasstoWaitForm(passedArgs);
                    }
                }
                else
                {
                    passedArgs.Messege = passedArgs.Messege + Environment.NewLine + "Could not Get Datsource";
                    Visutil.PasstoWaitForm(passedArgs);
                }
                Visutil.CloseWaitForm();
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

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }
        #endregion"Other Methods"
    }
}
