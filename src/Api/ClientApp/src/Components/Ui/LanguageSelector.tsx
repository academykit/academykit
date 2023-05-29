import React from "react";
import { LANGUAGES } from "../../constants";
import { useTranslation } from "react-i18next";
import { Select } from "@mantine/core";

const LanguageSelector = () => {
  const { i18n, t } = useTranslation();
  const lang = localStorage.getItem("lang");
  return (
    <Select
      value={lang ?? i18n.language}
      w={110}
      mr={5}
      onChange={(value) => {
        localStorage.setItem("lang", value as string);
        i18n.changeLanguage(value as string);
      }}
      data={LANGUAGES.map(({ code, label }) => {
        return {
          label,
          value: code,
        };
      })}
    />
  );
};

export default LanguageSelector;
