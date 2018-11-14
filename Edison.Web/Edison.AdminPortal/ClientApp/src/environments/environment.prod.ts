export const environment = {
    production: true,
    mockData: false,
    authorize: true,
    mapDefaults: {
        zoom: 16,
        style: {
            elements: {
                road: {
                    strokeColor: "#5b5b5b",
                    fillColor: "#ffffff"
                },
                controlledAccessHighway: {
                    strokeColor: "#5b5b5b",
                    fillColor: "#5b5b5b",
                },
                highway: {
                    strokeColor: "#5b5b5b",
                    fillColor: "#5b5b5b",
                },
                tollRoad: {
                    strokeColor: "#5b5b5b",
                    fillColor: "#5b5b5b",
                },
                education: {
                    fillColor: "#ffffff",
                }
            },
            settings: {
                landColor: "#E4E4E4",
            },
            version: "1.0"
        }
    },
    azureAd: {
        clientId: '2373be1e-6d0b-4e38-9115-e0bd01dadd61',
        authority: 'https://login.microsoftonline.com/',
        tenant: '1114b48d-24b1-4492-970a-d07d610a741c'
    },
    apiUrl: '/api/',
    baseUrl: 'https://edisonapi.eastus.cloudapp.azure.com',
    signalRUrl: '/signalr/',
    chatAuthUrl: '/chat/security/gettoken/',
    bingMapsKey:
        'Akt7a75JIqQ-QV2ZzHVP76eivabKNvlcq_JtF8zeTePsI38tt0LdAtAFeyh1MBrz'
};
