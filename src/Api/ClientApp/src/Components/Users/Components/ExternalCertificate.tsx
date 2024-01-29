import {
  ActionIcon,
  Badge,
  Box,
  Image,
  Paper,
  ScrollArea,
  Table,
  Text,
  Title,
  Tooltip,
} from '@mantine/core';
import { IconDownload, IconEye } from '@tabler/icons';
import { DATE_FORMAT } from '@utils/constants';
import downloadImage from '@utils/downloadImage';
import {
  GetExternalCertificate,
  useGetUserCertificate,
} from '@utils/services/certificateService';
import { TFunction } from 'i18next';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';

const RowsExternal = ({
  item,
  t,
}: {
  item: GetExternalCertificate;
  t: TFunction;
}) => {
  return (
    <Table.Tr key={item?.user?.id}>
      <Table.Td>{item?.name}</Table.Td>

      <Table.Td>
        {item?.status === 2 ? (
          <Badge color="cyan">{t('yes')}</Badge>
        ) : (
          <Badge color="cyan">{t('No')}</Badge>
        )}
      </Table.Td>
      <Table.Td>{moment(item?.startDate).format(DATE_FORMAT)}</Table.Td>
      <Table.Td>{moment(item?.endDate).format(DATE_FORMAT)}</Table.Td>
      <Table.Td>
        {item?.duration} {t('hour_s')}
      </Table.Td>
      <Table.Td>Rs.{item?.optionalCost}</Table.Td>
      <Table.Td>{item?.institute}</Table.Td>
      <Table.Td style={{ wordBreak: 'break-all' }}>{item.location}</Table.Td>
      <Table.Td>
        <Box style={{ width: 150, marginTop: 'auto', marginBottom: 'auto' }}>
          {item?.imageUrl ? (
            <div style={{ position: 'relative' }}>
              <Image
                width={150}
                height={100}
                fit="contain"
                style={{ opacity: '0.5' }}
                src={item?.imageUrl}
              />
              <div
                style={{
                  position: 'absolute',
                  left: 0,
                  bottom: 0,
                  right: 0,
                  margin: 'auto',
                  top: 0,
                  width: '45px',
                  height: '30px',
                  display: 'flex',
                }}
              >
                <Tooltip label={t('view_certificate')}>
                  <ActionIcon
                    onClick={() => window.open(item?.imageUrl)}
                    mr={10}
                    variant="subtle"
                  >
                    <IconEye color="black" />
                  </ActionIcon>
                </Tooltip>
                <Tooltip label={t('download_certificate')}>
                  <ActionIcon
                    variant="subtle"
                    onClick={() =>
                      downloadImage(item?.imageUrl, item?.user?.fullName ?? '')
                    }
                  >
                    <IconDownload color="black" />
                  </ActionIcon>
                </Tooltip>
              </div>
            </div>
          ) : (
            <Text>{t('no_certificate')}</Text>
          )}
        </Box>
      </Table.Td>
    </Table.Tr>
  );
};
const ExternalCertificate = () => {
  const { id } = useParams();
  const externalCertificate = useGetUserCertificate(id as string);
  const { t } = useTranslation();

  return (
    <>
      {externalCertificate.data && externalCertificate.data.length > 0 && (
        <>
          <Title mt={'xl'}>{t('external_certificate')}</Title>
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
                    <Table.Th>{t('verified')}</Table.Th>
                    <Table.Th>{t('start_date')}</Table.Th>
                    <Table.Th>{t('end_date')}</Table.Th>
                    <Table.Th>{t('duration')}</Table.Th>
                    <Table.Th>{t('optional_cost')}</Table.Th>
                    <Table.Th>{t('issued_by')}</Table.Th>
                    <Table.Th>{t('issuer_location')}</Table.Th>
                    <Table.Th>{t('external_certificate')}</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {externalCertificate?.data.map((x: any) => (
                    <RowsExternal key={x.id} item={x} t={t} />
                  ))}
                </Table.Tbody>
              </Table>
            </Paper>
          </ScrollArea>
        </>
      )}
    </>
  );
};

export default ExternalCertificate;
