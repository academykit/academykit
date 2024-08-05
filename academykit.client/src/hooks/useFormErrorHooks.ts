import { UseFormReturnType } from "@mantine/form";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";

const useFormErrorHooks = (form: UseFormReturnType<any>) => {
  const { i18n } = useTranslation();
  useEffect(() => {
    i18n.on("languageChanged", () => {
      Object.keys(form.errors).forEach((fieldName) => {
        form.validateField(fieldName);
      });
    });
    return () => {
      i18n.off("languageChanged", () => {});
    };
  }, [form.errors]);
};

export default useFormErrorHooks;
