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
        clientId: '5310587f-5da6-409a-8590-9e40ac7423f3',
        authority: 'https://login.microsoftonline.com/',
        tenant: '6c637512-c417-4e78-9d62-b61258e4b619'
    },
    apiUrl: '/api/',
    baseUrl: 'https://edisonapi.eastus.cloudapp.azure.com',
    signalRUrl: '/signalr/',
    chatAuthUrl: '/chat/security/gettoken/',
    bingMapsKey:
        'Akt7a75JIqQ-QV2ZzHVP76eivabKNvlcq_JtF8zeTePsI38tt0LdAtAFeyh1MBrz'
};
