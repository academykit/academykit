import ReactMarkdown from "react-markdown";
import { Container } from "@mantine/core";
import { useTranslation } from "react-i18next";

export const TermsPage = () => {
  const { t } = useTranslation();

  return (
    <Container>
      <ReactMarkdown children={t("terms_and_condition")} />
    </Container>
  );
};
export default TermsPage;
