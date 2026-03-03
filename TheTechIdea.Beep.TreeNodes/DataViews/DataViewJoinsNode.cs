using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    /// <summary>
    /// Displays all FederatedJoinDefinitions for an entity (or the entire view) as a sub-tree.
    /// Children are leaf nodes showing "Left.Col → Right.Col [JoinType]".
    /// </summary>
    [AddinAttribute(Caption = "Relations", BranchType = EnumPointType.JoinLeaf,
        Name = "DataViewJoinsNode.Beep", misc = "Beep",
        iconimage = "relations.png", menu = "Beep", ObjectType = "Beep")]
    public class DataViewJoinsNode : IBranch
    {
        public DataViewJoinsNode() { }

        public DataViewJoinsNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode,
            string parentEntityName, string dataViewSourceName, int pID)
        {
            TreeEditor      = pTreeEditor;
            DMEEditor       = pDMEEditor;
            ParentBranch    = pParentNode;
            ParentBranchID  = pParentNode?.ID ?? -1;
            DataSourceName  = dataViewSourceName;
            ParentEntityName = parentEntityName;
            BranchText      = "Relations";
            BranchType      = EnumPointType.Entity;
            IconImageName   = "relations.png";
            if (pID != 0) { ID = pID; BranchID = pID; }

            ds = DMEEditor.GetDataSource(dataViewSourceName) as DataViewDataSource;
        }

        // ── IBranch Properties ────────────────────────────────────────────────
        public string MenuID                   { get; set; }
        public bool   Visible                  { get; set; } = true;
        public bool   IsDataSourceNode         { get; set; } = true;
        public string GuidID                   { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID             { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID             { get; set; }
        public string MiscStringID             { get; set; }
        public IBranch ParentBranch            { get; set; }
        public string ObjectType               { get; set; } = "Beep";
        public int    ID                       { get; set; }
        public EntityStructure EntityStructure { get; set; } = new EntityStructure();
        public string Name                     { get; set; }
        public string BranchText               { get; set; } = "Relations";
        public IDMEEditor DMEEditor            { get; set; }
        public IDataSource DataSource          { get; set; }
        public string DataSourceName           { get; set; }
        public int    Level                    { get; set; }
        public EnumPointType BranchType        { get; set; } = EnumPointType.Entity;
        public int    BranchID                 { get; set; }
        public string IconImageName            { get; set; } = "relations.png";
        public string BranchStatus             { get; set; }
        public int    ParentBranchID           { get; set; }
        public string BranchDescription        { get; set; }
        public string BranchClass              { get; set; } = "DATAVIEW";
        public List<IBranch> ChildBranchs      { get; set; } = new List<IBranch>();
        public ITree  TreeEditor               { get; set; }
        public List<string> BranchActions      { get; set; } = new List<string>();
        public object TreeStrucure             { get; set; }
        public IAppManager Visutil             { get; set; }
        public int    MiscID                   { get; set; }

        /// <summary>The entity whose joins this node represents. Empty = all joins in the view.</summary>
        public string ParentEntityName { get; set; }

        DataViewDataSource ds;

        // ── IBranch Methods ───────────────────────────────────────────────────
        public IBranch CreateCategoryNode(CategoryFolder p) => throw new NotImplementedException();

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                if (ds == null) return DMEEditor.ErrorObject;

                var joins = string.IsNullOrWhiteSpace(ParentEntityName)
                    ? ds.DataView?.JoinDefinitions ?? new List<FederatedJoinDefinition>()
                    : ds.GetJoinsFor(ParentEntityName);

                foreach (var j in joins)
                {
                    string joinType = j.JoinType.ToString().ToUpper().Replace("INNER","⟶").Replace("LEFT","↖").Replace("RIGHT","↗").Replace("FULL","⟺");
                    string leafText = $"{j.LeftEntityName}.{j.LeftColumn}  {joinType}  {j.RightEntityName}.{j.RightColumn}";
                    string icon     = j.IsManuallyDefined ? "linkicon.png" : "relations.png";

                    var leaf = new DataViewEntityNode(TreeEditor, DMEEditor, this,
                        leafText, TreeEditor.SeqID, EnumPointType.Field, icon, DataSourceName);
                    leaf.MiscStringID = j.GuidID; // store join GUID for Edit/Remove
                    ChildBranchs.Add(leaf);
                    TreeEditor.Treebranchhandler.AddBranch(this, leaf);
                }

                DMEEditor.AddLogMessage("Info", $"Relations node expanded: {joins.Count} joins.", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "DataViewJoinsNode.CreateChildNodes failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName) => DMEEditor.ErrorObject;
        public IErrorsInfo MenuItemClicked(string ActionNam)      => DMEEditor.ErrorObject;
        public IErrorsInfo RemoveChildNodes()
        {
            TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
            ChildBranchs.Clear();
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode,
            string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor     = pTreeEditor;
                DMEEditor      = pDMEEditor;
                ParentBranchID = pParentNode?.ID ?? -1;
                BranchText     = pBranchText;
                BranchType     = pBranchType;
                IconImageName  = pimagename;
                if (pID != 0) { ID = pID; BranchID = pID; }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "DataViewJoinsNode.SetConfig failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        // ── Commands ──────────────────────────────────────────────────────────

        [CommandAttribute(Caption = "Add Relation", iconimage = "add.png", ObjectType = "Beep")]
        public IErrorsInfo AddRelation()
        {
            try
            {
                if (ds == null) return DMEEditor.ErrorObject;
                var ob = new List<ObjectItem>
                {
                    new ObjectItem { Name = "Branch",        obj = this },
                    new ObjectItem { Name = "DataView",      obj = ds.DataView },
                    new ObjectItem { Name = "LeftEntity",    obj = ParentEntityName },
                    new ObjectItem { Name = "TitleText",     obj = $"Add Relation — {ParentEntityName}" }
                };
                var args = new PassedArgs
                {
                    DMView         = ds.DataView,
                    ObjectName     = DataSourceName,
                    ObjectType     = "ADDJOIN",
                    EventType      = "ADDJOIN",
                    DatasourceName = DataSourceName,
                    Objects        = ob
                };
                Visutil.ShowPage("uc_RelationBuilder", args);
                // Refresh after dialog closes
                RemoveChildNodes();
                CreateChildNodes();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "AddRelation failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Clear Manual Relations", iconimage = "remove.png", ObjectType = "Beep")]
        public IErrorsInfo ClearManualRelations()
        {
            try
            {
                if (ds == null) return DMEEditor.ErrorObject;
                if (Visutil.DialogManager.InputBoxYesNoAsync("Clear Manual Relations",
                    "Remove all manually-defined joins from this view?").GetAwaiter().GetResult().Result == BeepDialogResult.Yes)
                {
                    ds.ClearManualJoins();
                    ds.WriteDataViewFile(DataSourceName);
                    RemoveChildNodes();
                    CreateChildNodes();
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "ClearManualRelations failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Validate Relations", iconimage = "validate.png", ObjectType = "Beep")]
        public IErrorsInfo ValidateRelations()
        {
            try
            {
                if (ds == null) return DMEEditor.ErrorObject;
                var errors = ds.ValidateJoins();
                Visutil.DialogManager.MsgBoxAsync("Validate Relations",
                    errors.Count == 0
                        ? "✔ All joins are valid."
                        : string.Join(Environment.NewLine, errors)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "ValidateRelations failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Auto-Detect Joins", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo AutoDetect()
        {
            try
            {
                if (ds == null) return DMEEditor.ErrorObject;
                ds.AutoDetectJoins();
                ds.WriteDataViewFile(DataSourceName);
                RemoveChildNodes();
                CreateChildNodes();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "AutoDetect failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
    }
}
