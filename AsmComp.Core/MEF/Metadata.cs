using AsmComp.Core.Hierarchy;

namespace AsmComp.Core.MEF;

public interface IAssemblyComposer {
}
public interface IDotNetMetadataComparer {
    HierarchicalDirectory? Compare(object x, object y);
}
public interface IExporter {
}
public interface ISheetExporter {
}
public interface IParser {
}
public interface IAcText {
}
