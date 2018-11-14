export interface MapClick {
    getX: () => number;
    getY: () => number;
    eventName: string;
    isPrimary: boolean;
    isSecondary: boolean;
    layer: any;
    location: {
        longitude: number;
        latitude: number;
    }
}
