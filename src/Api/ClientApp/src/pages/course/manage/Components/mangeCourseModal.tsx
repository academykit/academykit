import { Modal } from '@mantine/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

const ManageCourseModal = ({
  opened,
  setOpened,
}: {
  opened: boolean;
  setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { t } = useTranslation();
  return (
    <Modal opened={opened} onClose={() => setOpened(false)}>
      {t('hello')}
    </Modal>
  );
};

export default ManageCourseModal;
