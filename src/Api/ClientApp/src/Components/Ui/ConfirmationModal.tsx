/* eslint-disable */
import { Button, Group, Modal } from '@mantine/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

const ConfirmationModal = ({
  onClose,
  title,
  open,
  onConfirm,
}: {
  onClose: Function;
  title: string;
  open: boolean;
  onConfirm: Function;
}) => {
  const { t } = useTranslation();
  return (
    <Modal title={title} opened={open} onClose={() => onClose()}>
      <Group mt={10}>
        <Button
          onClick={() => {
            onConfirm();
            onClose();
          }}
        >
          {t('confirm')}
        </Button>
        <Button variant="outline" onClick={() => onClose()}>
          {t('cancel')}
        </Button>
      </Group>
    </Modal>
  );
};

export default ConfirmationModal;
