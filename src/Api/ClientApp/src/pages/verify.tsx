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
          {t("confirmation_sent")}
        </Text>
        <Text>{t("check_inbox")}</Text>
        <Text>
          {t("email_vurilo")}
          <a href="mailto:support@vurilo.com">{t("email_address_vurilo")}</a>
        </Text>
        <Text mt={10}>{t("valid_five_minutes")}</Text>
        <Button mt={20} loading={resendEmail.isLoading} onClick={handleClick}>
          {t("resend_verification")}
        </Button>
      </div>
    </Container>
  );
};

export default Verify;
