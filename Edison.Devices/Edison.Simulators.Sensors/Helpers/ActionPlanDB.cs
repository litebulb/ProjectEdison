using Edison.Common.DAO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Simulators.Sensors.Helpers
{
    public class ActionPlanDB
    {
        public static List<ActionPlanDAO> GetActionPlans()
        {
            List<ActionPlanDAO> actionPlans = new List<ActionPlanDAO>();

            var actionPlanFire = new ActionPlanDAO()
            {
                Id = "e768ab86-64d4-42e0-bd8a-9f012f774bbf",
                Name = "Fire",
                Description = "Pre-configured action plan for a fire",
                IsActive = true,
                Color = "red",
                Icon = "fire",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateEmergencyCallMessage(),
                     GenerateNotificationMessage("A fire has been detected in the area. If you are inside, listen for alarms instructing you to go outside."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The fire has been contained. Safe to proceed as directed by authorities on the ground."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };
            var actionPlanActiveShooter = new ActionPlanDAO()
            {
                Id = "32d4bc56-3c39-4000-9355-c08068a2ba1a",
                Name = "Active Shooter",
                Description = "Pre-configured action plan for an active shooter",
                IsActive = true,
                Color = "red",
                Icon = "gun",
                PrimaryRadius = 2,
                SecondaryRadius = 3,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateEmergencyCallMessage(),
                     GenerateNotificationMessage("Active Shooter in the area. Proceed with action plan immediately."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("Active Shooter situation ended. More info to come."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };
            var actionPlanAirQuality = new ActionPlanDAO()
            {
                Id = "5524c361-540f-424f-8a56-23165fb515c6",
                Name = "Air Quality",
                Description = "Pre-configured action plan for an air quality",
                IsActive = true,
                Color = "yellow",
                Icon = "pollution",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("Air Quality issues have been reported in the area.")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("Air Quality issues in the area have improved.")
                }
            };
            var actionPlanHealthCheck = new ActionPlanDAO()
            {
                Id = "9ffe839c-1341-4a3b-9d33-bafd5e6cf407",
                Name = "Health Check",
                Description = "Pre-configured action plan for a health check",
                IsActive = true,
                Color = "blue",
                Icon = "health",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("Warning. There might be a strong virus in your area.")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The health check situation was resolved.")
                }
            };
            var actionPlanActiveProtest = new ActionPlanDAO()
            {
                Id = "a79815c0-274a-4794-b7f0-175f9e48b257",
                Name = "Protest",
                Description = "Pre-configured action plan for a protest",
                IsActive = true,
                Color = "blue",
                Icon = "protest",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = false,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("A planned protest will occur today in the area. Travel routes may be affected."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The planned protest has concluded."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };
            var actionPlanActiveSuspiciousPackage = new ActionPlanDAO()
            {
                Id = "c06d7a38-c51d-4e7f-9704-17686dd9f304",
                Name = "Suspicious Package",
                Description = "Pre-configured action plan for a suspicious package",
                IsActive = true,
                Color = "red",
                Icon = "package",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = false,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("A suspicious package has been detected near your location."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The suspicious package situation has been resolved."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };
            var actionPlanActiveTornado = new ActionPlanDAO()
            {
                Id = "2cbcfb49-3348-4b49-878f-8cee71a18940",
                Name = "Tornado",
                Description = "Pre-configured action plan for a tornado",
                IsActive = true,
                Color = "yellow",
                Icon = "tornado",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("A tornado watch is in effect for the area."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The tornado watch has expired."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };
            var actionPlanVIP = new ActionPlanDAO()
            {
                Id = "aff4de61-6f3d-45a2-9e27-81dda79a19e0",
                Name = "VIP",
                Description = "Pre-configured action plan for vip",
                IsActive = true,
                Color = "blue",
                Icon = "vip",
                PrimaryRadius = 1,
                SecondaryRadius = 2,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("There is a planned appearance by a public figure in the area today. Travel routes may be affected.")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("The planned appearance by a public figure has concluded.")
                }
            };
            var actionPlanActiveEmergency = new ActionPlanDAO()
            {
                Id = "0bbe4f8e-fe45-4c85-8d59-f4c3031823a5",
                Name = "Emergency",
                Description = "Pre-configured action plan for emergency",
                IsActive = true,
                Color = "red",
                Icon = "fire",
                PrimaryRadius = 2,
                SecondaryRadius = 3,
                AcceptSafeStatus = true,
                OpenActions = new List<ActionDAOObject>()
                {
                     GenerateEmergencyCallMessage(),
                     GenerateNotificationMessage("Emergency."),
                     GenerateActionPrimaryLightMessage("red"),
                     GenerateActionSecondaryLightMessage("yellow")
                },
                CloseActions = new List<ActionDAOObject>()
                {
                     GenerateNotificationMessage("End of emergency."),
                     GenerateActionPrimaryLightMessage("off"),
                     GenerateActionSecondaryLightMessage("off")
                }
            };

            actionPlans.Add(actionPlanFire);
            actionPlans.Add(actionPlanActiveShooter);
            actionPlans.Add(actionPlanAirQuality);
            actionPlans.Add(actionPlanHealthCheck);
            actionPlans.Add(actionPlanActiveProtest);
            actionPlans.Add(actionPlanActiveSuspiciousPackage);
            actionPlans.Add(actionPlanActiveTornado);
            actionPlans.Add(actionPlanVIP);
            actionPlans.Add(actionPlanActiveEmergency);

            return actionPlans;
        }

        private static ActionDAOObject GenerateActionPrimaryLightMessage(string color)
        {
            return new ActionDAOObject()
            {
                ActionType = "lightsensor",
                IsActive = 1,
                Description = "Connected Lights Primary Radius",
                Parameters = new Dictionary<string, string>()
                 {
                     { "radius", "primary" },
                     { "flashfrequency", "0" },
                     { "color", color },
                     { "state", "on" }
                 }
            };
        }

        private static ActionDAOObject GenerateActionSecondaryLightMessage(string color)
        {
            return new ActionDAOObject()
            {
                ActionType = "lightsensor",
                IsActive = 1,
                Description = "Connected Lights Secondary Radius",
                Parameters = new Dictionary<string, string>()
                 {
                     { "radius", "secondary" },
                     { "flashfrequency", "0" },
                     { "color", color },
                     { "state", "on" }
                 }
            };
        }

        private static ActionDAOObject GenerateNotificationMessage(string message)
        {
            return new ActionDAOObject()
            {
                ActionType = "notification",
                IsActive = 1,
                Description = "Mobile App Notification will be sent",
                Parameters = new Dictionary<string, string>()
                 {
                     { "message", message },
                     { "issilent", "True" }
                 }
            };
        }

        //this will need to be updated after trellio support
        private static ActionDAOObject GenerateEmergencyCallMessage()
        {
            return new ActionDAOObject()
            {
                ActionType = "rapidsos",
                IsActive = 1,
                Description = "911 will be called by RapidSOS",
                Parameters = new Dictionary<string, string>()
                 {
                     { "servicetype", "rapidsos" },
                     { "message", "Message to RapidSOS" }
                 }
            };
        }
    }
}
