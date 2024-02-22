import AIStar from '@components/Icons/AIStar';
import {
  ActionIcon,
  Flex,
  Group,
  Input,
  Loader,
  Popover,
  Text,
  TextInput,
  Textarea,
  Tooltip,
} from '@mantine/core';
import { IconReload, IconThumbUp } from '@tabler/icons';
import { t } from 'i18next';
import Copy from '../Copy';

interface IAIProps {
  title: string | undefined;
  description: string | undefined;
  isLoading: boolean;
  refetch: () => void;
  acceptAnswer: () => void;
}

const TitleAndDescriptionSuggestion = ({
  title,
  description,
  isLoading,
  refetch,
  acceptAnswer,
}: IAIProps) => {
  return (
    <Flex gap={5} align={'center'}>
      <Text>{t('powered_by_ai')}</Text>

      <Popover width={400} trapFocus position="bottom" withArrow shadow="md">
        <Popover.Target>
          <ActionIcon variant="subtle" c={'gray'}>
            <AIStar fontSize={18} />
          </ActionIcon>
        </Popover.Target>
        <Popover.Dropdown>
          <TextInput
            readOnly
            label={
              <Flex align={'center'} gap={3}>
                <Input.Label>{t('title')}</Input.Label>
                <Copy value={title ?? ''} disabled={isLoading} />
              </Flex>
            }
            placeholder={t('ai_title_suggestion') as string}
            rightSection={isLoading && <Loader size={16} />}
            value={title}
          />

          <Textarea
            readOnly
            rightSection={isLoading && <Loader size={16} />}
            mt={10}
            label={
              <Flex align={'center'} gap={3}>
                <Input.Label>{t('description')}</Input.Label>
                <Copy value={description ?? ''} disabled={isLoading} />
              </Flex>
            }
            placeholder={t('ai_description_suggestion') as string}
            autosize
            minRows={2}
            maxRows={4}
            value={description}
          />

          <Group gap="xs" mt={10}>
            <Tooltip label={t('regenerate_suggestion')}>
              <ActionIcon
                variant="transparent"
                c={'gray'}
                disabled={isLoading}
                onClick={refetch}
              >
                <IconReload size={18} />
              </ActionIcon>
            </Tooltip>

            <Tooltip label={t('accept_answer')}>
              <ActionIcon
                variant="transparent"
                c={'gray'}
                onClick={acceptAnswer}
                disabled={isLoading}
              >
                <IconThumbUp size={18} />
              </ActionIcon>
            </Tooltip>
          </Group>
        </Popover.Dropdown>
      </Popover>
    </Flex>
  );
};

export default TitleAndDescriptionSuggestion;
