import { Button, Group, Modal } from '@mantine/core';
import { useTranslation } from 'react-i18next';

const ConfirmationModal = ({
  onClose,
  title,
  open,
  onConfirm,
}: {
  onClose: () => void;
  title: string;
  open: boolean;
  onConfirm: () => void;
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
