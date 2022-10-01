export interface ConnectionStatusChangedModel {
    reconnected?:boolean,
    isReconnecting?:boolean,
    disconnected?:boolean,
    failedToConnect?:boolean,
    connectedByJs?:boolean,
    //invoke is missing because it is from c#
}