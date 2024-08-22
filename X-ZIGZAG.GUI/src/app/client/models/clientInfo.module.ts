import { IpInfo } from "./ipdata.module";

export interface ClientInfo {
    id: string;
    customName?: string;
    name: string;
    created: Date;
    latestUpdate: Date;
    latestPing: Date;
    checkDuration:number;
    ipAddress: string;
    version: string;
    systemSpecs: string;
    location?:IpInfo
  }
  