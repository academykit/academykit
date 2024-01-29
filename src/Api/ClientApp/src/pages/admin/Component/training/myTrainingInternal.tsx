import {
  ActionIcon,
  Box,
  Card,
  Container,
  Flex,
  Image,
  Text,
  Tooltip,
} from '@mantine/core';
import { IconDownload, IconEye } from '@tabler/icons';
import { DATE_FORMAT } from '@utils/constants';
import downloadImage from '@utils/downloadImage';
import { useGetInternalCertificate } from '@utils/services/certificateService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

const MyTrainingInternal = () => {
  const internal = useGetInternalCertificate();
  const { t } = useTranslation();
  return (
    <Container fluid>
      {internal.isSuccess && internal.data.data.length <= 0 && (
        <Box>{t('no_certificates')} </Box>
      )}
      {internal.isSuccess &&
        internal.data.data.map((x, i) => (
          <Card key={i} withBorder mt={10}>
            <Flex justify={'space-between'}>
              <Box>
                <Text fw={'bold'}>{x.courseName}</Text>
                <Text fw={'bold'}>
                  {t('certificate_issue_date')}{' '}
                  {moment(x.certificateIssuedDate).format(DATE_FORMAT)}
                </Text>
                <Text>
                  {t('total')} {x.percentage}
                  {t('percent_completed')}
                </Text>
              </Box>
              <Box
                style={{ width: 150, marginTop: 'auto', marginBottom: 'auto' }}
              >
                {x.certificateUrl && (
                  <div style={{ position: 'relative' }}>
                    <Image
                      src={x.certificateUrl || ''}
                      radius="md"
                      style={{
                        opacity: '0.5',
                      }}
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
                          onClick={() => window.open(x.certificateUrl)}
                          mr={10}
                          variant="subtle"
                        >
                          <IconEye color="black" />
                        </ActionIcon>
                      </Tooltip>
                      <Tooltip label={t('download_certificate')}>
                        <ActionIcon
                          variant="subtle"
                          onClick={() => {
                            downloadImage(
                              x?.certificateUrl ?? '',
                              x?.courseName ?? ''
                            );
                          }}
                        >
                          <IconDownload color="black" />
                        </ActionIcon>
                      </Tooltip>
                    </div>
                  </div>
                )}
              </Box>
            </Flex>
          </Card>
        ))}
    </Container>
  );
};

export default MyTrainingInternal;
