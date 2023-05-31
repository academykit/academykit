import useAuth from "@hooks/useAuth";
import { Button, Container, Text } from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { useResendEmailVerification } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import React from "react";
import { useTranslation } from "react-i18next";
import { useSearchParams } from "react-router-dom";

const Verify = () => {
  const auth = useAuth();
  const resendEmail = useResendEmailVerification();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const { t } = useTranslation();
  const handleClick = async () => {
    try {
      await resendEmail.mutateAsync({ token: token as string });
      showNotification({
        title: t("successful"),
        message: t("re_sent_email"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };
  return (
    <Container>
      <div style={{ marginTop: "150px" }}>
        <Text mb={10} size={"xl"} weight="bold">
          Confirmation Email has been sent
        </Text>
        <Text>Please check your new email inbox to confirm your process.</Text>
        <Text>
          If you have any trouble, you can email support@vurilo.com for help at
          any time.
        </Text>
        <Text mt={10}>
          Note: Current Email change request is only valid for 5 Minutes
        </Text>
        <Button mt={20} loading={resendEmail.isLoading} onClick={handleClick}>
          Resend Verification Email
        </Button>
      </div>
    </Container>
  );
};

export default Verify;
