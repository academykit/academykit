import { Select } from '@mantine/core';
import { useTranslation } from 'react-i18next';
import { LANGUAGES } from '../../constants';

const LanguageSelector = () => {
  const { i18n } = useTranslation();
  const lang = localStorage.getItem('lang');
  return (
    <Select
      allowDeselect={false}
      value={lang ?? i18n.language}
      w={110}
      mr={5}
      onChange={(value) => {
        localStorage.setItem('lang', value as string);
        i18n.changeLanguage(value as string);
        window.location.reload();
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
