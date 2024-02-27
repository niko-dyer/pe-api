import logo from "../../../assets/images/PE_logo_with_name_no_background.png";
import "./LoginForm.scss";
import { AuthenticationAPI } from "../../../apis/AuthenticationApi";
import { useNavigate } from "react-router-dom";
import { UserAPI } from "../../../apis/UserApi";

import {
  Avatar,
  Button,
  TextField,
  Link,
  Grid,
  Box,
  Typography,
  Container,
  Snackbar,
  AlertTitle,
  Alert,
} from "@mui/material";
import { useState } from "react";

export default function SignIn() {
  const [openSnackBar, setOpenSnackBar] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [displayMessage, setDisplayMessage] = useState("");

  const navigate = useNavigate();
  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const email = data.get("email") as string;
    const password = data.get("password") as string;
    if (email !== "" && password !== "") {
      AuthenticationAPI.login(email, password).then((response) => {
        if (response.status === 200) {
          UserAPI.getCurrentUser().then((response) => {
            localStorage.setItem("accountData", JSON.stringify(response));
            navigate("/", { replace: true });
          });
        } else {
          setIsSuccess(false);
          setDisplayMessage(
            "Unable to login. Please check your credentials and try again."
          );
          setOpenSnackBar(true);
          console.error(response);
        }
      });
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Avatar
          sx={{
            m: 1,
            border: "1px solid black",
            height: "100px",
            width: "100px",
          }}
          src={logo}
        ></Avatar>
        <Typography component="h1" variant="h5">
          Sign in
        </Typography>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Sign In
          </Button>
          <Grid container>
            <Grid item xs>
              <Link href="/forgot-password" variant="body2">
                Forgot password?
              </Link>
            </Grid>
          </Grid>
        </Box>
      </Box>
      <Snackbar
        open={openSnackBar}
        autoHideDuration={5000}
        onClose={() => setOpenSnackBar(false)}
      >
        <Alert
          severity={isSuccess ? "success" : "error"}
          sx={{ whiteSpace: "pre-line" }}
          onClose={() => setOpenSnackBar(false)}
        >
          <AlertTitle>{isSuccess ? "Success" : "Error"}</AlertTitle>
          {displayMessage}
        </Alert>
      </Snackbar>
    </Container>
  );
}
