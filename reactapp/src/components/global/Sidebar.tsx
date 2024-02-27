import DialpadIcon from "@mui/icons-material/Dialpad";
import EditNoteIcon from "@mui/icons-material/EditNote";
import GroupIcon from "@mui/icons-material/Group";
import CampaignIcon from "@mui/icons-material/Campaign";
import AssistWalkerIcon from "@mui/icons-material/AssistWalker";
import { Link } from "react-router-dom";
import "../../assets/scss/Sidebar.scss";
import logo from "./../../assets/images/PE_crutch_logo-removebg-preview.png";
import { useState } from "react";
import React from "react";
import DeviceHubIcon from "@mui/icons-material/DeviceHub";
import {
  Drawer,
  Button,
  List,
  Divider,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box,
} from "@mui/material";
import { User } from "../../models/User";
import { hasRequiredRoleForPage } from "../Shared/ProtectedRoute";

type Anchor = "top" | "left" | "bottom" | "right";

interface SideBarLink {
  link: string;
  title: string;
  icon: JSX.Element;
}

const accountData: User = JSON.parse(
  localStorage.getItem("accountData") || "{}"
);

function filterLinksByRole(role: string, links: SideBarLink[]): SideBarLink[] {
  return links.filter((sideBarLink) => {
    return hasRequiredRoleForPage(role, sideBarLink?.link);
  });
}

const LinkSection = (sectionTitle: string, sideBarLinks: SideBarLink[]) => {
  const filteredLinks = filterLinksByRole(accountData?.role, sideBarLinks);
  return (
    filteredLinks.length > 0 && (
      <>
        <h3 className="sidebar-head">{sectionTitle}</h3>
        <Divider />
        <List>
          {Object.entries(filteredLinks).map(([key, value]) => (
            <ListItem component={Link} to={value.link} key={key} disablePadding>
              <ListItemButton>
                <ListItemIcon>{value.icon}</ListItemIcon>
                <ListItemText className="sidebar-link" primary={value.title} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </>
    )
  );
};

export default function Sidebar() {
  const [state, setState] = useState({
    top: false,
    left: false,
    bottom: false,
    right: false,
  });

  const toggleDrawer =
    (anchor: Anchor, open: boolean) =>
    (event: React.KeyboardEvent | React.MouseEvent) => {
      if (
        event.type === "keydown" &&
        ((event as React.KeyboardEvent).key === "Tab" ||
          (event as React.KeyboardEvent).key === "Shift")
      ) {
        return;
      }

      setState({ ...state, [anchor]: open });
    };

  const inventoryLinks: SideBarLink[] = [
    {
      link: "/",
      title: "View Inventory",
      icon: <AssistWalkerIcon />,
    },
    {
      link: "/inventory/add",
      title: "Update Inventory",
      icon: <EditNoteIcon />,
    },
  ];

  const userLinks: SideBarLink[] = [
    {
      link: "/contact/list",
      title: "Contact List",
      icon: <DialpadIcon />,
    },
    {
      link: "/users/list",
      title: "User List",
      icon: <GroupIcon />,
    },
  ];

  const campaignLinks: SideBarLink[] = [
    {
      link: "/campaign/list",
      title: "Campaign List",
      icon: <CampaignIcon />,
    },
  ];

  const deviceLinks: SideBarLink[] = [
    {
      link: "/device/list",
      title: "Device List",
      icon: <DeviceHubIcon />,
    },
  ];

  const list = (anchor: Anchor) => (
    <Box
      sx={{ width: anchor === "top" || anchor === "bottom" ? "auto" : 250 }}
      role="presentation"
      onClick={toggleDrawer(anchor, false)}
      onKeyDown={toggleDrawer(anchor, false)}
    >
      {LinkSection("Inventory Management", inventoryLinks)}
      {LinkSection("User Management", userLinks)}
      {LinkSection("Campaign Management", campaignLinks)}
      {LinkSection("Device Management", deviceLinks)}
    </Box>
  );

  return (
    <div>
      <React.Fragment key={"left"}>
        <Button>
          <img
            src={logo}
            alt="Projet Embrace Logo"
            onClick={toggleDrawer("left", true)}
          />
        </Button>
        <Drawer
          anchor="left"
          open={state["left"]}
          onClose={toggleDrawer("left", false)}
        >
          {list("left")}
        </Drawer>
      </React.Fragment>
    </div>
  );
}
