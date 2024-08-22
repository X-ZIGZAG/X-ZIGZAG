// Represents a single screenshot
export interface Screenshot {
    screenshotId: string; 
}

// Represents a single screen
export interface Screen {
    id: number; 
    screenshots?: Screenshot[]; 
}
export interface Screens {
    screens?: Screen[]; 
    duration?: number; 
}
