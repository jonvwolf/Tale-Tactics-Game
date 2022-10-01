import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from "../node_modules/@microsoft/signalr/dist/esm/index";
import { ConnectionStatusChangedModel } from "./ConnectionStatusChanged";
import { GameCodeModel } from "./GameCodeModel";
import { HmCommandModel } from "./HmCommandModel";
import { HmPredefinedCommandModel } from "./HmPredefinedCommandModel";
import { HtHubListeners } from "./hthublisteners";
import { HtUnityInstance } from "./htunityinstance";
import { TextLogModel } from "./TextLogModel";

const hubTries = [10, 10, 15];

export class HtHub {

    private unity = new HtUnityInstance();
    private hub:HubConnection|null = null;
    private listeners:HtHubListeners|null = null;

    private url:string = '';
    private gameCode:GameCodeModel = {gameCode:''};
    
    constructor(){
    }

    public setVars(url:string, gameCode:string, unityInstance:any):void {
        this.url = url;
        this.gameCode = {gameCode: gameCode};

        if(unityInstance === undefined || unityInstance === null){
            console.error('UnityInstance is null or undefined')
        }
        // TODO: create an interface for this unityInstance
        this.unity.unity = unityInstance;
    }

    public connect():void {
        
        if(this.hub !== null){
            this.stop();
        }else{
            // just in case
            this.reset();
        }

        this.hub = new HubConnectionBuilder()
            .withUrl(this.url, {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect(hubTries)
            .build();

        this.hub.start().then(() => {
            if(this.listeners !== null){
                // just in case
                this.listeners.unregister();
            }
            // just in case
            this.unregisterOnCallbacks();

            if(this.hub === null){
                // This could happen when calling connect then stop before this success/failure executes
                console.warn('Hub connected but somehow hub variable is null...');
                return;
            }

            this.listeners = new HtHubListeners(this.hub, this.unity, this.gameCode);
            this.registerOnCallbacks();

            console.log('Connected to Hub OK');
            this.hub.invoke('JoinGameAsPlayer', this.gameCode)
                .then(() => {
                    const connected:ConnectionStatusChangedModel = {
                        connectedByJs: true
                    };
                    this.unity.OnConnectionStatusChanged(connected);
                    console.log('CONNECTED TO HUB gamecode: ', '`' + this.gameCode.gameCode + '`');
                    const msg:TextLogModel = {
                        from: 'A player',
                        message: 'Has joined'
                    };
                    this.hub?.send('PlayerSendLog', this.gameCode, msg)
                        .then(() => { console.log('Successfully sent Player log'); })
                        .catch((err) => {
                            console.warn('Error sending PlayerSendLog', err);
                        });
                })
                .catch((err) => {
                    console.error('Error invoking JoinGameAsPlayer', err);
                    // This has to be stopped because other functionality won't work
                    this.stop();
                });
        }).catch((err) => {
            console.error('Error trying to connect', err);
            this.unity.OnConnectionStatusChanged({
                failedToConnect: true
            });
            this.reset();
        });
    }

    private reset():HubConnection|null{
        this.listeners?.unregister();
        this.listeners = null;
        this.unregisterOnCallbacks();

        const localHub = this.hub;
        this.hub = null;
        return localHub;
    }

    public stop():void {
        const localHub = this.reset();
        if(localHub !== null){
            if(localHub.state === HubConnectionState.Connected){
                const msg:TextLogModel = {
                    from: 'A player',
                    message: 'Has left'
                };
                localHub.send('PlayerSendLog', this.gameCode, msg)
                    .catch((err) => {
                        console.warn('Error sending PlayerSendLog', err);
                    });
            }
            
            if(localHub.state !== HubConnectionState.Disconnected){
                console.log('Calling stop on hub...');

                // if `stop` doesn't stop the `then` of `start`, while connecting for example, then
                // `then` of start will execute, but because there are hub null checks, it will not register callbacks, etc.
                localHub.stop().finally(() => {
                    // TODO: should I raise event that is disconnected?
                    console.log('Hub connection closed OK');
                }).catch((err) => {
                    console.warn('Error trying to close', err);
                });
            }else{
                console.log('Didn\'t call stop on hub because it is already disconnected');
            }
        }
    }

    private registerOnCallbacks(){
        if(this.hub === null){
            console.error('Hub is null');
            return;
        }

        console.log('Registered on callbacks for hub');
        this.hub.on('PlayerReceiveHmCommand', (data) => {
            console.log('Received from hub', data);
            const xdata = data as HmCommandModel;
            this.unity.OnHmCommand(xdata);

            this.hub?.send('PlayerSendBackHmCommand', this.gameCode, xdata)
                .catch((err)=>{
                    console.warn('Error sending PlayerSendBackHmCommand', err);
                });
        });

        this.hub.on('PlayerReceiveHmCommandPredefined', (data) => {
            console.log('Received from hub-predefined', data);
            const xdata = data as HmPredefinedCommandModel;
            this.unity.OnHmPredefinedCommand(xdata);

            this.hub?.send('PlayerSendBackHmCommandPredefined', this.gameCode, xdata)
                .catch((err) => {
                    console.warn('Error sending PlayerSendBackHmCommandPredefined', err);
                });
        });
    }

    private unregisterOnCallbacks(){
        try{
            if(this.hub !== null){
                console.log('Unregistered on callbacks for hub');
                this.hub.off('PlayerReceiveHmCommand');
                this.hub.off('PlayerReceiveHmCommandPredefined');
            }
        }catch(error){
            console.error('Error at unregister callbacks', error);
        }
    }

    public dispose(): void {
        this.stop();
    }
}