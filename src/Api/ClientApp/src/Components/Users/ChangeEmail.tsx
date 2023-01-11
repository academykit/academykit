import { Alert, Button, Container } from "@mantine/core";
import { IconAlertCircle, IconTrophy } from "@tabler/icons";
import { useVerifyChangeEmail } from "@utils/services/authService";
import React, { useEffect } from "react";
import { Link, useSearchParams } from "react-router-dom";

const ChangeEmail = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const verifyChangeEmail = useVerifyChangeEmail(token as string);

  return (
    <Container size="sm">
      <div style={{ marginTop: "150px" }}>
        {verifyChangeEmail.isSuccess && (
          <Alert icon={<IconTrophy size={20} />} title="Success" color="green">
            Your email has been changed successfully.
          </Alert>
        )}

        {verifyChangeEmail.isError && (
          <Alert
            icon={<IconAlertCircle size={20} />}
            title="Error!"
            color="pink"
          >
            Something terrible happened! Some error occurred while changing
            email!
          </Alert>
        )}
        <Button mt={20} variant={"outline"} component={Link} to="/">
          Go Back to Home
        </Button>
      </div>
    </Container>
  );
};

export default ChangeEmail;
