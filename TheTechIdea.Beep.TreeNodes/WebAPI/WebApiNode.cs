﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Addin;


namespace TheTechIdea.Beep.TreeNodes.WebAPI
{
    [AddinAttribute(Caption = "Web API", BranchType = EnumPointType.DataPoint, Name = "WebApiNode.Beep", misc = "Beep", iconimage = "webapi.png", menu = "Beep", ObjectType = "Beep")]
    public class WebApiNode : IBranch 
    {
        private ConnectionProperties _conn;

        public WebApiNode(ConnectionProperties cn, ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            IconImageName = pimagename;
            DataSourceName = pBranchText;
            if (pID != 0)
            {
                ID = pID;
                BranchID = ID;
            }
            _conn = cn;
            EntityGuidID = _conn.GuidID;
        }
        public bool Visible { get; set; } = true;
        public string MenuID { get; set; }
        public bool IsDataSourceNode { get; set; } = true;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public IDataSource DataSource { get ; set ; }
        public string DataSourceName { get ; set ; }
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public EntityStructure EntityStructure { get; set; }
        public int MiscID { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.DataPoint;
        public int BranchID { get; set; }
        public string IconImageName { get; set; }
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "WEBAPI";
        public object TreeStrucure { get ; set ; }
        public  IAppManager  Visutil  { get ; set ; }
        

        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {

            try
            {

                CreateWebApiEntitiesAsync();

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

        [CommandAttribute(Caption = "Get Entities", iconimage = "getchilds.png", ObjectType = "Beep")]
        public  IErrorsInfo CreateViewEntitesAsync()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            DMEEditor.Logger.WriteLog($"Filling View Entities Web Api");
            try
            {
                bool loadv = false;
                if (ChildBranchs.Count > 0)
                {
                    if (Visutil.DialogManager.InputBoxYesNo("Beep", "Do you want to over write th existing View Structure?") ==BeepDialogResult.Yes)
                    {
                       TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                       loadv = true;
                    }
                }
                else
                {
                    loadv = true;
                }
                if (loadv)
                {
                   CreateWebApiEntitiesAsync();
                }
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Web Api Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }

        [CommandAttribute(Caption = "Edit Web API", Name = "EditWebApi", iconimage = "createwebapi.png")]
        public IErrorsInfo EditWebApi()
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
                    ObjectType = "WEBAPI",
                    DataSource = null,
                    ObjectName = BranchText,
                    ParameterString1 =_conn.GuidID,
                    Objects = ob,

                    DatasourceName = BranchText,
                    EventType = "EDITWEBAPI"

                };
                // ActionNeeded?.Invoke(this, Passedarguments);
                Visutil.ShowPage("uc_WebApi", Passedarguments);



            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Database Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }
        #endregion Exposed Interface"
        #region "Other Methods"

        public async Task<bool> CreateWebApiEntitiesAsync()
        {
            try

            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                WebApiEntities webent;
                List<EntityStructure> ls = new List<EntityStructure>();
                 DataSource = DMEEditor.GetDataSource(BranchText);
               
                if (DataSource != null)
                {
                    DataSource.GetEntitesList();
                    ls = DMEEditor.ConfigEditor.DataConnections.Where(i => i.ConnectionName == BranchText).FirstOrDefault().Entities;
                    if (ls != null)
                    {
                        foreach (EntityStructure item in ls)
                        {

                            if (!DataSource.Entities.Where(o => o.EntityName.Equals(item.EntityName, StringComparison.OrdinalIgnoreCase)).Any())
                            {
                                DataSource.Entities.Add(item);
                            }
                        }

                    }
                    if (DataSource.Entities != null)
                    {
                        if (DataSource.Entities.Count > 0)
                        {
                            
                            List<EntityStructure> rootent = DataSource.Entities.Where(i => i.ParentId == 0).ToList();
                           // TreeEditor.ShowWaiting();
                           
                              CreateEntitiesJob(rootent); 
                           
                          //  TreeEditor.HideWaiting();
                        }
                    }

                }






                DMEEditor.AddLogMessage("Success", $"Generated WebApi node", DateTime.Now, 0, null, Errors.Ok);
                return true;
            }
            catch (Exception ex)
            {
                string errmsg = "Error in Generating App Version";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);
                return false;
            }


        }
        private void CreateEntitiesJob(List<EntityStructure> rootent)
        {
           
            int cnt = rootent.Count;
            int startcnt = 1;
           // TreeEditor.ChangeWaitingCaption($"Getting Web Api Entities/Categories Total:{cnt}");
            foreach (EntityStructure item in rootent)
            {
                string iconimage = "webapi.png";
                EnumPointType branchType = EnumPointType.Entity;
                if (item.Category == "Category")
                {
                    iconimage = "webapicategory.png";
                    branchType = EnumPointType.DataPoint;
                }
                WebApiEntities webentmain = new WebApiEntities(TreeEditor, DMEEditor, this, item.EntityName, TreeEditor.SeqID, branchType, iconimage, DataSourceName);
                webentmain.DataSource = DataSource;
                webentmain.DataSourceName = DataSource.DatasourceName;

                TreeEditor.Treebranchhandler.AddBranch(this, webentmain);
                TreeEditor.AddBranchToParentInBranchsOnly(this,webentmain);
              //  TreeEditor.AddCommentsWaiting($"{startcnt} - Adding {item.EntityName} to WebAPI DataSource");
                startcnt +=1;
                CreateNode(DataSource.Entities, item, webentmain);

            }
        }
        private void CreateNode(List<EntityStructure> entities,EntityStructure parententity,IBranch br)
        {
            try
            {
               
                List<EntityStructure> ls = entities.Where(i => i.ParentId == parententity.Id).ToList();
                WebApiEntities webent;
                foreach (var item in ls)
                {
                    string iconimage = "webapi.png";
                    EnumPointType branchType = EnumPointType.Entity;
                    if (item.Category == "Category")
                    {
                        iconimage = "webapicategory.png";
                        branchType = EnumPointType.DataPoint;
                    }
                    webent = new WebApiEntities(TreeEditor, DMEEditor, br, item.EntityName, TreeEditor.SeqID, branchType, iconimage, BranchText);
                    webent.DataSource = DataSource;
                    webent.DataSourceName = DataSource.DatasourceName;
                    TreeEditor.Treebranchhandler.AddBranch(br, webent);
                    TreeEditor.AddBranchToParentInBranchsOnly(this,webent);
                  //  TreeEditor.AddCommentsWaiting($"{startcnt} - Adding {item.EntityName} to WebAPI DataSource");
                    if (entities.Where(i => i.ParentId == item.Id && i.Id != 0).Any())
                    {
                        CreateNode(entities, item,webent);
                    }
                }
               
            }
            catch (Exception ex)
            {

                string errmsg = "Error in creating nodes for WebAPI";
                DMEEditor.AddLogMessage("Fail", $"{errmsg}:{ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
        }
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

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }

        #endregion"Other Methods"

    }
}
