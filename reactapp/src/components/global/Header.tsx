import { Component, lazy } from "react";
import "../../assets/scss/Header.scss";
import { Link } from "react-router-dom";
import {
  Breadcrumbs,
  Typography,
  AppBar,
  Box,
  Toolbar,
  Button,
} from "@mui/material";
import { AuthenticationAPI } from "../../apis/AuthenticationApi.js";

const Sidebar = lazy(() => import("./Sidebar.js") as any);

const logout = () => {
  localStorage.removeItem("accountData");
  AuthenticationAPI.logout();
};
export default class Header extends Component<{ title: string }> {
  render() {
    return (
      <>
        <Box sx={{ flexGrow: 1 }}>
          <AppBar position="static">
            <Toolbar>
              <Sidebar />
              <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                Project Embrace
              </Typography>
              <a href="/login">
                <Button color="inherit" onClick={logout}>
                  Logout
                </Button>
              </a>
            </Toolbar>
          </AppBar>
        </Box>
        <div className="page-heading">
          <Typography variant="h1">{this.props.title}</Typography>
          <Breadcrumbs aria-label="breadcrumb">
            <Link color="inherit" to="/">
              Root
            </Link>
            <Link
              color="inherit"
              to="/material-ui/getting-started/installation/"
            >
              Previous
            </Link>
            <Typography color="text.primary">Current</Typography>
          </Breadcrumbs>
        </div>
      </>
    );
  }
}
