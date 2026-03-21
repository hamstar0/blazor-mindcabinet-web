namespace MindCabinet.Shared.DataObjects;


public interface IDataObject {
    // This class tries to validate all its members, and also tries to give foreign data members (e.g. UserContext in UserAppDataObject)
    // as much data as possible, so that the client doesn't have to make additional API calls to get the data it needs.
}


public interface IRawDataObject {
    // This is the raw data object that is used for (de)serialization. It should not contain any logic or validation.
}