import TextViewer from "@components/Ui/RichTextViewer";
import { Text } from "@mantine/core";
import { useTranslation } from "react-i18next";

const VariableHelpTable = () => {
  const { t } = useTranslation();

  const TemplateViewer = ({
    title,
    placeholder,
    example,
  }: {
    title: string;
    placeholder: string;
    example: string;
  }) => {
    return (
      <>
        <Text>{t(title)}</Text>
        <Text>{t(placeholder)}</Text>
        <TextViewer content={t(example)} />
      </>
    );
  };

  return (
    <>
      <Text>{t("template_welcome")}</Text>
      <TemplateViewer
        title="template_1"
        placeholder="template_1_placeholder"
        example="template_1_eg"
      />
      <br />
      <TemplateViewer
        title="template_2"
        placeholder="template_2_placeholder"
        example="template_2_eg"
      />
      <br />
      <TemplateViewer
        title="template_3"
        placeholder="template_3_placeholder"
        example="template_3_eg"
      />
      <br />
      <TemplateViewer
        title="template_4"
        placeholder="template_4_placeholder"
        example="template_4_eg"
      />
      <br />
      <TemplateViewer
        title="template_5"
        placeholder="template_5_placeholder"
        example="template_5_eg"
      />
      <br />
      <TemplateViewer
        title="template_6"
        placeholder="template_6_placeholder"
        example="template_6_eg"
      />
      <br />
      <TemplateViewer
        title="template_7"
        placeholder="template_7_placeholder"
        example="template_7_eg"
      />
      <br />
      <TemplateViewer
        title="template_8"
        placeholder="template_8_placeholder"
        example="template_8_eg"
      />
      <br />
      <TemplateViewer
        title="template_9"
        placeholder="template_9_placeholder"
        example="template_9_eg"
      />
      <br />
      <TemplateViewer
        title="template_10"
        placeholder="template_10_placeholder"
        example="template_10_eg"
      />
      <br />
      <TemplateViewer
        title="template_11"
        placeholder="template_11_placeholder"
        example="template_11_eg"
      />
      <br />
      <TemplateViewer
        title="template_12"
        placeholder="template_12_placeholder"
        example="template_12_eg"
      />
      <br />
      <TemplateViewer
        title="template_13"
        placeholder="template_13_placeholder"
        example="template_13_eg"
      />
      <br />
      <TemplateViewer
        title="template_14"
        placeholder="template_14_placeholder"
        example="template_14_eg"
      />
      <br />
      <TemplateViewer
        title="template_15"
        placeholder="template_15_placeholder"
        example="template_15_eg"
      />
      <br />
      <TemplateViewer
        title="template_16"
        placeholder="template_16_placeholder"
        example="template_16_eg"
      />
    </>
  );
};

export default VariableHelpTable;
