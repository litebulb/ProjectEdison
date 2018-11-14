import { Device } from "../../../reducers/device/device.model";

export interface FilterModel {
    title: string;
    iconClasses?: string;
    onClick: (checked: boolean) => any;
    checked: boolean;
}
