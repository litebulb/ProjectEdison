export namespace SignalRTypes {

    export enum Channel {
        Device = 'UpdateDeviceUI',
        Event = 'UpdateEventClusterUI',
        Response = 'UpdateResponseUI',
    }

    export enum Event {
        New = 'NewEventCluster',
        Update = 'UpdateEventCluster',
        Close = 'CloseEventCluster',
    }

    export enum Device {
        Update = 'UpdateDevice',
        New = 'NewDevice',
        Delete = 'DeleteDevice',
    }

    export enum Response {
        Update = 'UpdateResponse',
        New = 'NewResponse',
        Close = 'CloseResponse',
    }

}
