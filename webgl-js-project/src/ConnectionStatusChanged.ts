export interface ConnectionStatusChangedModel {
    reconnected?:boolean,
    isReconnecting?:boolean,
    disconnected?:boolean,
    failedToConnect?:boolean
    //invoke is missing because it is from c#
}