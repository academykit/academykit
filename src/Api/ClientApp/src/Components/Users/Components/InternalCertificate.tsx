import {
  ActionIcon,
  Anchor,
  Badge,
  Box,
  CopyButton,
  Flex,
  Image,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Title,
  Tooltip,
} from '@mantine/core';
import { IconCheck, IconDownload, IconEye } from '@tabler/icons-react';
import { useProfileAuth } from '@utils/services/authService';
import { ICertificateList } from '@utils/services/manageCourseService';
import { TFunction } from 'i18next';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';

const RowsCompleted = ({
  item,
  t,
}: {
  item: ICertificateList;
  t: TFunction;
}) => {
  const [opened, setOpened] = useState(false);
  const handleDownload = () => {
    window.open(item?.certificateUrl);
  };
  return (
    <Table.Tr key={item?.user?.id}>
      <Table.Td>{item?.courseName}</Table.Td>

      <Table.Td>{item?.percentage}%</Table.Td>
      <Table.Td>
        {item?.hasCertificateIssued ? (
          <Badge color="cyan">{t('yes')}</Badge>
        ) : (
          <Badge color="cyan">{t('no')}</Badge>
        )}
      </Table.Td>
      <Table.Td style={{ width: '19%', height: '100px' }}>
        <Modal
          opened={opened}
          size="xl"
          title={item?.courseName}
          onClose={() => setOpened(false)}
        >
          <Image src={item?.certificateUrl}></Image>
        </Modal>
        <div style={{ position: 'relative', width: '150px', height: '100px' }}>
          <Anchor onClick={() => setOpened((v) => !v)}>
            <Image
              width={150}
              height={100}
              fit="contain"
              // style={{":hover"}}
              src={item?.certificateUrl}
            />
          </Anchor>
          <Flex
            justify={'center'}
            align={'center'}
            style={{
              position: 'absolute',
              left: 0,
              bottom: 0,
              right: 0,
              top: 0,
              width: '100%',
              height: '100%',
            }}
          >
            <CopyButton value={item?.certificateUrl} timeout={2000}>
              {({ copied, copy }) => (
                <Tooltip
                  label={copied ? t('copied') : t('copy')}
                  withArrow
                  position="right"
                >
                  <ActionIcon
                    variant="subtle"
                    color={copied ? 'teal' : 'gray'}
                    onClick={copy}
                  >
                    {copied ? (
                      <IconCheck size={18} color="black" />
                    ) : (
                      <IconEye size={18} color="black" />
                    )}
                  </ActionIcon>
                </Tooltip>
              )}
            </CopyButton>
            <ActionIcon variant="subtle" onClick={() => handleDownload()}>
              <IconDownload color="black" />
            </ActionIcon>
          </Flex>
        </div>
      </Table.Td>
    </Table.Tr>
  );
};

const InternalCertificate = () => {
  const { id } = useParams();
  const { data } = useProfileAuth(id as string);
  const { t } = useTranslation();
  return (
    <>
      <Title mt={'xl'}>{t('internal_certificate')}</Title>
      {data && data?.certificates.length > 0 ? (
        <ScrollArea>
          <Paper mt={10}>
            <Table
              style={{ minWidth: 800 }}
              verticalSpacing="sm"
              striped
              highlightOnHover
              withTableBorder
              withColumnBorders
            >
              <Table.Thead>
                <Table.Tr>
                  <Table.Th>{t('training_name')}</Table.Th>
                  <Table.Th>{t('completion')}</Table.Th>
                  <Table.Th>{t('is_issued')}</Table.Th>
                  <Table.Th>{t('certificate_url')}</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {data?.certificates &&
                  data?.certificates.map((x: any) => (
                    <RowsCompleted key={x.userId} item={x} t={t} />
                  ))}
              </Table.Tbody>
            </Table>
          </Paper>
        </ScrollArea>
      ) : (
        <Box mt={10}>{t('no_certificate')}</Box>
      )}
    </>
  );
};

export default InternalCertificate;
