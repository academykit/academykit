/* eslint-disable */
import { Button, Group, Modal } from '@mantine/core';
import { useTranslation } from 'react-i18next';

const DeleteModal = ({
  onClose,
  title,
  open,
  onConfirm,
  loading = false,
}: {
  onClose: () => void;
  title: string;
  open: boolean;
  onConfirm: () => void;
  loading?: boolean;
}) => {
  const { t } = useTranslation();
  return (
    <Modal
      styles={{ header: { alignItems: 'start' } }}
      title={title}
      opened={open}
      onClose={() => onClose()}
      zIndex={301}
    >
      <Group mt={10}>
        <Button
          loading={loading}
          onClick={() => {
            onConfirm();
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

export default DeleteModal;
