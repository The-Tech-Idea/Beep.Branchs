# How To Guide: TheTechIdea.Beep.TreeNodes

This guide provides comprehensive instructions for implementing, extending, and using tree nodes in the Beep Enterprise framework.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Creating Custom Nodes](#creating-custom-nodes)
3. [Node Patterns](#node-patterns)
4. [Integration Examples](#integration-examples)
5. [Advanced Scenarios](#advanced-scenarios)
6. [Troubleshooting](#troubleshooting)

## Getting Started

### Prerequisites

Ensure you have the required dependencies:

```xml
<PackageReference Include="TheTechIdea.Beep.DataManagementEngine" Version="2.0.43" />
<PackageReference Include="TheTechIdea.Beep.Vis.Modules" Version="2.0.24" />
```

### Basic Node Structure

Every tree node must implement the `IBranch` interface:

```csharp
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;

[AddinAttribute(Caption = "My Node", misc = "Beep", BranchType = EnumPointType.Entity, iconimage = "mynode.png")]
public class MyCustomNode : IBranch
{
    // Core properties
    public string GuidID { get; set; } = Guid.NewGuid().ToString();
    public string BranchText { get; set; }
    public EnumPointType BranchType { get; set; }
    public string IconImageName { get; set; }
    public IDMEEditor DMEEditor { get; set; }
    public ITree TreeEditor { get; set; }
    public IBranch ParentBranch { get; set; }
    
    // Constructor
    public MyCustomNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                        string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
    {
        TreeEditor = pTreeEditor;
        DMEEditor = pDMEEditor;
        ParentBranch = pParentNode;
        BranchText = pBranchText;
        BranchType = pBranchType;
        IconImageName = pimagename;
        ID = pID;
    }
    
    // Required interface implementations
    public IErrorsInfo CreateChildNodes() { /* Implementation */ }
    public IErrorsInfo ExecuteBranchAction(string ActionName) { /* Implementation */ }
    public IErrorsInfo MenuItemClicked(string ActionName) { /* Implementation */ }
    public IErrorsInfo RemoveChildNodes() { /* Implementation */ }
    public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                                string pBranchText, int pID, EnumPointType pBranchType, string pimagename) { /* Implementation */ }
}
```

## Creating Custom Nodes

### Step 1: Define Node Attributes

Use the `AddinAttribute` to define node metadata:

```csharp
[AddinAttribute(
    Caption = "Custom Data Source",           // Display name
    misc = "MyExtension",                     // Extension/module name
    BranchType = EnumPointType.DataPoint,    // Node type
    iconimage = "custom-datasource.png",     // Icon file
    menu = "Data",                           // Menu category
    ObjectType = "DataSource"                // Object classification
)]
```

### Step 2: Implement Core Methods

#### CreateChildNodes()
Populate child nodes based on your data source:

```csharp
public IErrorsInfo CreateChildNodes()
{
    try
    {
        // Clear existing children
        ChildBranchs.Clear();
        
        // Get data from your source
        var entities = GetEntitiesFromDataSource();
        
        foreach (var entity in entities)
        {
            var childNode = new CustomEntityNode(
                TreeEditor, DMEEditor, this, 
                entity.Name, TreeEditor.SeqID, 
                EnumPointType.Entity, "entity.png");
                
            TreeEditor.Treebranchhandler.AddBranch(this, childNode);
        }
        
        DMEEditor.AddLogMessage("Success", "Created child nodes", DateTime.Now, 0, null, Errors.Ok);
    }
    catch (Exception ex)
    {
        DMEEditor.AddLogMessage("Error", $"Failed to create child nodes: {ex.Message}", 
                               DateTime.Now, -1, null, Errors.Failed);
    }
    
    return DMEEditor.ErrorObject;
}
```

#### ExecuteBranchAction()
Handle node-specific actions:

```csharp
public IErrorsInfo ExecuteBranchAction(string ActionName)
{
    switch (ActionName.ToUpper())
    {
        case "REFRESH":
            return RefreshNode();
        case "CONNECT":
            return ConnectToDataSource();
        case "DISCONNECT":
            return DisconnectFromDataSource();
        default:
            return DMEEditor.ErrorObject;
    }
}
```

### Step 3: Add Context Menu Commands

Use the `CommandAttribute` for context menu actions:

```csharp
[CommandAttribute(Caption = "Refresh Data", iconimage = "refresh.png")]
public IErrorsInfo RefreshData()
{
    try
    {
        // Refresh logic
        RemoveChildNodes();
        CreateChildNodes();
        
        DMEEditor.AddLogMessage("Success", "Data refreshed", DateTime.Now, 0, null, Errors.Ok);
    }
    catch (Exception ex)
    {
        DMEEditor.AddLogMessage("Error", $"Refresh failed: {ex.Message}", 
                               DateTime.Now, -1, null, Errors.Failed);
    }
    
    return DMEEditor.ErrorObject;
}

[CommandAttribute(Caption = "Export Schema", iconimage = "export.png")]
public IErrorsInfo ExportSchema()
{
    // Export implementation
    return DMEEditor.ErrorObject;
}
```

## Node Patterns

### Pattern 1: Root Container Node

Root nodes serve as top-level containers:

```csharp
[AddinAttribute(Caption = "My System", BranchType = EnumPointType.Genre, iconimage = "system.png")]
public class MySystemRootNode : IBranch
{
    public MySystemRootNode()
    {
        BranchText = "My System";
        BranchClass = "MYSYSTEM.ROOT";
        BranchType = EnumPointType.Genre;
        IconImageName = "system.png";
    }
    
    public IErrorsInfo CreateChildNodes()
    {
        // Create category nodes
        var categoryNode = new MyCategoryNode(TreeEditor, DMEEditor, this, 
                                            "Category 1", TreeEditor.SeqID, 
                                            EnumPointType.Category, "category.png");
        TreeEditor.Treebranchhandler.AddBranch(this, categoryNode);
        
        return DMEEditor.ErrorObject;
    }
}
```

### Pattern 2: Category Grouping Node

Category nodes organize related items:

```csharp
public class MyCategoryNode : IBranch
{
    public IErrorsInfo CreateChildNodes()
    {
        // Load items for this category
        var items = GetCategoryItems();
        
        foreach (var item in items)
        {
            var itemNode = new MyItemNode(TreeEditor, DMEEditor, this, 
                                         item.Name, TreeEditor.SeqID, 
                                         EnumPointType.DataPoint, "item.png");
            itemNode.DataObject = item; // Store reference
            TreeEditor.Treebranchhandler.AddBranch(this, itemNode);
        }
        
        return DMEEditor.ErrorObject;
    }
}
```

### Pattern 3: Data Entity Node

Entity nodes represent actual data objects:

```csharp
public class MyItemNode : IBranch
{
    public object DataObject { get; set; } // Store the actual data item
    
    [CommandAttribute(Caption = "View Details", iconimage = "view.png")]
    public IErrorsInfo ViewDetails()
    {
        // Show detailed view of the data object
        var viewer = DMEEditor.GetAddin("DataViewer") as IDisplayManager;
        viewer?.ShowData(DataObject);
        
        return DMEEditor.ErrorObject;
    }
    
    [CommandAttribute(Caption = "Edit Properties", iconimage = "edit.png")]
    public IErrorsInfo EditProperties()
    {
        // Open property editor
        return DMEEditor.ErrorObject;
    }
}
```

## Integration Examples

### Example 1: Custom Database Integration

```csharp
[AddinAttribute(Caption = "Custom DB", BranchType = EnumPointType.DataPoint, iconimage = "customdb.png")]
public class CustomDatabaseNode : IBranch
{
    public IDataSource DataSource { get; set; }
    
    public IErrorsInfo CreateChildNodes()
    {
        try
        {
            if (DataSource?.ConnectionStatus == ConnectionState.Open)
            {
                // Get tables/entities
                DataSource.GetEntitesList();
                
                foreach (string entityName in DataSource.EntitiesNames)
                {
                    var entityNode = new CustomTableNode(TreeEditor, DMEEditor, this, 
                                                        entityName, TreeEditor.SeqID, 
                                                        EnumPointType.Entity, "table.png");
                    entityNode.DataSource = DataSource;
                    entityNode.EntityName = entityName;
                    
                    TreeEditor.Treebranchhandler.AddBranch(this, entityNode);
                }
            }
        }
        catch (Exception ex)
        {
            DMEEditor.AddLogMessage("Error", $"Failed to load entities: {ex.Message}", 
                                   DateTime.Now, -1, null, Errors.Failed);
        }
        
        return DMEEditor.ErrorObject;
    }
    
    [CommandAttribute(Caption = "Connect", iconimage = "connect.png")]
    public IErrorsInfo Connect()
    {
        try
        {
            DataSource?.Openconnection();
            if (DataSource?.ConnectionStatus == ConnectionState.Open)
            {
                CreateChildNodes();
                DMEEditor.AddLogMessage("Success", "Connected successfully", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
        }
        catch (Exception ex)
        {
            DMEEditor.AddLogMessage("Error", $"Connection failed: {ex.Message}", 
                                   DateTime.Now, -1, null, Errors.Failed);
        }
        
        return DMEEditor.ErrorObject;
    }
}
```

### Example 2: File System Integration

```csharp
public class CustomFileNode : IBranch
{
    public string FilePath { get; set; }
    
    public IErrorsInfo CreateChildNodes()
    {
        if (Path.GetExtension(FilePath).ToLower() == ".zip")
        {
            // Create nodes for zip contents
            CreateZipEntryNodes();
        }
        else if (IsDataFile(FilePath))
        {
            // Create sheet/table nodes for data files
            CreateDataSheetNodes();
        }
        
        return DMEEditor.ErrorObject;
    }
    
    private void CreateDataSheetNodes()
    {
        try
        {
            var dataSource = DMEEditor.GetDataSource(Path.GetFileName(FilePath));
            if (dataSource != null)
            {
                dataSource.GetEntitesList();
                
                foreach (string sheetName in dataSource.EntitiesNames)
                {
                    var sheetNode = new FileEntitySheetNode(TreeEditor, DMEEditor, this, 
                                                           sheetName, TreeEditor.SeqID, 
                                                           EnumPointType.Entity, "sheet.png");
                    TreeEditor.Treebranchhandler.AddBranch(this, sheetNode);
                }
            }
        }
        catch (Exception ex)
        {
            DMEEditor.AddLogMessage("Error", $"Failed to load file sheets: {ex.Message}", 
                                   DateTime.Now, -1, null, Errors.Failed);
        }
    }
}
```

## Advanced Scenarios

### Lazy Loading

Implement lazy loading for large data sets:

```csharp
public class LazyLoadingNode : IBranch
{
    private bool _childrenLoaded = false;
    
    public IErrorsInfo CreateChildNodes()
    {
        if (!_childrenLoaded)
        {
            LoadChildrenAsync();
            _childrenLoaded = true;
        }
        
        return DMEEditor.ErrorObject;
    }
    
    private async Task LoadChildrenAsync()
    {
        // Async loading logic
        await Task.Run(() => {
            var children = GetLargeDataSet();
            
            Application.Invoke(() => {
                foreach (var child in children)
                {
                    // Add to tree on UI thread
                    var childNode = CreateChildNode(child);
                    TreeEditor.Treebranchhandler.AddBranch(this, childNode);
                }
            });
        });
    }
}
```

### Dynamic Icon Management

Change icons based on state:

```csharp
public class StatefulNode : IBranch
{
    public NodeState State { get; set; }
    
    public void UpdateState(NodeState newState)
    {
        State = newState;
        
        IconImageName = State switch
        {
            NodeState.Connected => "connected.png",
            NodeState.Disconnected => "disconnected.png",
            NodeState.Error => "error.png",
            NodeState.Loading => "loading.png",
            _ => "default.png"
        };
        
        // Trigger UI update
        TreeEditor.RefreshNode(this);
    }
}
```

### Context-Aware Actions

Actions that change based on context:

```csharp
public class ContextAwareNode : IBranch
{
    public IErrorsInfo ExecuteBranchAction(string ActionName)
    {
        // Check user permissions
        if (!DMEEditor.Security.HasPermission(ActionName, this))
        {
            DMEEditor.AddLogMessage("Warning", "Insufficient permissions", 
                                   DateTime.Now, 0, null, Errors.Failed);
            return DMEEditor.ErrorObject;
        }
        
        // Check node state
        if (State == NodeState.Busy)
        {
            DMEEditor.AddLogMessage("Warning", "Node is busy", 
                                   DateTime.Now, 0, null, Errors.Failed);
            return DMEEditor.ErrorObject;
        }
        
        return base.ExecuteBranchAction(ActionName);
    }
}
```

## Error Handling Best Practices

### Centralized Error Logging

```csharp
public void LogError(string operation, Exception ex)
{
    DMEEditor.AddLogMessage("Error", 
                           $"{operation} failed in {this.GetType().Name}: {ex.Message}", 
                           DateTime.Now, -1, BranchText, Errors.Failed);
    
    // Optional: Send to external logging system
    DMEEditor.Logger?.WriteLog($"Node Error: {operation} - {ex}");
}
```

### Graceful Degradation

```csharp
public IErrorsInfo CreateChildNodes()
{
    try
    {
        var primaryData = GetPrimaryDataSource();
        CreateNodesFromData(primaryData);
    }
    catch (Exception ex)
    {
        LogError("Primary data load", ex);
        
        // Fallback to cached or default data
        try
        {
            var cachedData = GetCachedData();
            CreateNodesFromData(cachedData);
            
            DMEEditor.AddLogMessage("Warning", "Using cached data", 
                                   DateTime.Now, 0, null, Errors.Ok);
        }
        catch (Exception cacheEx)
        {
            LogError("Cache fallback", cacheEx);
            
            // Create empty placeholder
            CreateEmptyPlaceholder();
        }
    }
    
    return DMEEditor.ErrorObject;
}
```

## Performance Optimization

### Node Pooling

```csharp
public static class NodePool
{
    private static readonly ConcurrentQueue<IBranch> _pool = new();
    
    public static T GetNode<T>() where T : IBranch, new()
    {
        if (_pool.TryDequeue(out var node) && node is T typedNode)
        {
            return typedNode;
        }
        
        return new T();
    }
    
    public static void ReturnNode(IBranch node)
    {
        // Reset node state
        node.ChildBranchs?.Clear();
        node.DataSource = null;
        
        _pool.Enqueue(node);
    }
}
```

### Virtualization

```csharp
public class VirtualizedNode : IBranch
{
    private readonly int _pageSize = 100;
    private int _currentPage = 0;
    
    public IErrorsInfo LoadNextPage()
    {
        var items = GetPagedData(_currentPage, _pageSize);
        
        foreach (var item in items)
        {
            var childNode = CreateChildNode(item);
            TreeEditor.Treebranchhandler.AddBranch(this, childNode);
        }
        
        _currentPage++;
        return DMEEditor.ErrorObject;
    }
}
```

## Troubleshooting

### Common Issues

1. **Icons not displaying**
   - Ensure icon files are marked as `EmbeddedResource`
   - Check icon filename matches exactly (case-sensitive)
   - Verify icon is in the correct format (PNG recommended)

2. **Context menu actions not appearing**
   - Verify `CommandAttribute` is correctly applied
   - Check method signature matches expected pattern
   - Ensure method is public

3. **Child nodes not loading**
   - Check `CreateChildNodes()` implementation
   - Verify data source connections
   - Look for exceptions in error logs

4. **Performance issues**
   - Implement lazy loading for large datasets
   - Use virtualization for very large collections
   - Consider node pooling for frequently created/destroyed nodes

### Debugging Tips

```csharp
public class DebuggableNode : IBranch
{
    public IErrorsInfo CreateChildNodes()
    {
        DMEEditor.AddLogMessage("Debug", $"Creating children for {BranchText}", 
                               DateTime.Now, 0, null, Errors.Ok);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            // Your logic here
            var result = DoCreateChildren();
            
            stopwatch.Stop();
            DMEEditor.AddLogMessage("Debug", 
                                   $"Child creation completed in {stopwatch.ElapsedMilliseconds}ms", 
                                   DateTime.Now, 0, null, Errors.Ok);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            DMEEditor.AddLogMessage("Error", 
                                   $"Child creation failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}", 
                                   DateTime.Now, -1, null, Errors.Failed);
            throw;
        }
    }
}
```

## Testing

### Unit Testing Nodes

```csharp
[Test]
public void TestNodeCreation()
{
    // Arrange
    var mockDMEEditor = new Mock<IDMEEditor>();
    var mockTreeEditor = new Mock<ITree>();
    
    // Act
    var node = new MyCustomNode(mockTreeEditor.Object, mockDMEEditor.Object, 
                               null, "Test Node", 1, EnumPointType.Entity, "test.png");
    
    // Assert
    Assert.AreEqual("Test Node", node.BranchText);
    Assert.AreEqual(EnumPointType.Entity, node.BranchType);
    Assert.AreEqual("test.png", node.IconImageName);
}

[Test]
public void TestChildNodeCreation()
{
    // Arrange
    var node = CreateTestNode();
    
    // Act
    var result = node.CreateChildNodes();
    
    // Assert
    Assert.AreEqual(Errors.Ok, result.Flag);
    Assert.IsTrue(node.ChildBranchs.Count > 0);
}
```

This guide covers the essential patterns and practices for working with Beep TreeNodes. For specific implementation details, refer to the existing node implementations in the codebase.