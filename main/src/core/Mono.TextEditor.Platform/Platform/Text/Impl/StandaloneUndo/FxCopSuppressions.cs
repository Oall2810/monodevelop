#if CODE_ANALYSIS_BASELINE
using System.Diagnostics.CodeAnalysis;
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="UndoCalledButCanUndoFalse", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#Undo()", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="DoCalledButCanRedoFalse", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#Do()", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="ClosingAnOpenTransactionThatAppearsToBeUndoneOrUndoing", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#Dispose()", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CompleteCalledOnTransationThatIsNotOpened", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#Complete()", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CancelCalledOnTransationThatIsNotOpened", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#Cancel()", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="AddUndoCalledOnTransationThatIsNotOpened", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#AddUndo(Microsoft.VisualStudio.Text.Operations.ITextUndoPrimitive)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="GetHistoryCannotFindContextInRegistry", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#GetHistory(System.Object)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="AttachHistoryAlreadyContainsContextInRegistry", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#AttachHistory(System.Object,Microsoft.VisualStudio.Text.Operations.ITextUndoHistory,System.Boolean)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CannotUndoRequestedPrimitiveFromHistoryUndo", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Undo(System.Int32)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CannotUndoMoreTransactionsThanExist", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Undo(System.Int32)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CannotUndoMoreTransactionsThanExist", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Redo(System.Int32)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CannotRedoRequestedPrimitiveFromHistoryRedo", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Redo(System.Int32)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="EndTransactionOutOfOrder", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#EndTransaction(Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="CannotCreateTransactionWhenCurrentTransactionNotOpen", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#CreateTransaction(System.String)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="NullMergePolicyCannotMerge", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.NullMergeUndoTransactionPolicy.#CompleteTransactionMerge(Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction,Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction,Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="DelegatedUndoPrimitiveCannotMerge", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.DelegatedUndoPrimitiveImpl.#Merge(Microsoft.VisualStudio.Text.Operations.ITextUndoPrimitive)", Justification="Member name")]
[module: SuppressMessage("Microsoft.Naming","CA2204:Literals should be spelled correctly", MessageId="DelegatedUndoPrimitiveStateDoesNotAllowAdd", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.DelegatedUndoPrimitiveImpl.#AddOperation(Microsoft.VisualStudio.Text.Operations.Standalone.UndoableOperationCurried)", Justification="Member name")]


//ToDo: To be looked at
[module: SuppressMessage("Microsoft.Design","CA1020:AvoidNamespacesWithFewTypes", Scope="namespace", Target="Microsoft.VisualStudio.Text.Operations.Standalone", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Reliability","CA2000:Dispose objects before losing scope", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#MergeOrPushToUndoStack(Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Performance","CA1822:MarkMembersAsStatic", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.DelegatedUndoPrimitiveImpl.#MergeWithPreviousOnly", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Performance","CA1822:MarkMembersAsStatic", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#ProceedWithMerge(Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl,Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#CreateTransaction(System.String)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Redo(System.Int32)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#Undo(System.Int32)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#AttachHistory(System.Object,Microsoft.VisualStudio.Text.Operations.ITextUndoHistory)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#AttachHistory(System.Object,Microsoft.VisualStudio.Text.Operations.ITextUndoHistory,System.Boolean)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#GetHistory(System.Object)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#RegisterHistory(System.Object)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#RegisterHistory(System.Object,System.Boolean)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#RemoveHistory(Microsoft.VisualStudio.Text.Operations.ITextUndoHistory)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryRegistryImpl.#TryGetHistory(System.Object,Microsoft.VisualStudio.Text.Operations.ITextUndoHistory&)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Usage","CA2241:Provide correct arguments to formatting methods", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#.ctor(Microsoft.VisualStudio.Text.Operations.ITextUndoHistory,Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction,System.String)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Performance","CA1804:RemoveUnusedLocals", MessageId="transaction", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#IsThereEnoughVisibleTransactions(System.Collections.Generic.Stack`1<Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction>,System.Int32)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Performance","CA1804:RemoveUnusedLocals", MessageId="registry", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#ProceedWithMerge(Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl,Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Design","CA1062:Validate arguments of public methods", MessageId="0", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoTransactionImpl.#AddUndo(Microsoft.VisualStudio.Text.Operations.ITextUndoPrimitive)", Justification="ToDo: To be looked at")]
[module: SuppressMessage("Microsoft.Maintainability","CA1500:VariableNamesShouldNotMatchFieldNames", MessageId="state", Scope="member", Target="Microsoft.VisualStudio.Text.Operations.Standalone.UndoHistoryImpl.#RaiseUndoRedoHappened(Microsoft.VisualStudio.Text.Operations.TextUndoHistoryState,Microsoft.VisualStudio.Text.Operations.ITextUndoTransaction)", Justification="ToDo: To be looked at")]

#endif