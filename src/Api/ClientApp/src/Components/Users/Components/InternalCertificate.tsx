import {
  ActionIcon,
  Anchor,
  Badge,
  CopyButton,
  Flex,
  Image,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Title,
  Tooltip,
  Box,
} from "@mantine/core";
import { IconCheck, IconDownload, IconEye } from "@tabler/icons";
import { useProfileAuth } from "@utils/services/authService";
import { ICertificateList } from "@utils/services/manageCourseService";
import { TFunction } from "i18next";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

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
    <tr key={item?.user?.id}>
      <td>{item?.courseName}</td>

      <td>{item?.percentage}%</td>
      <td>
        {item?.hasCertificateIssued ? (
          <Badge>{t("yes")}</Badge>
        ) : (
          <Badge>{t("no")}</Badge>
        )}
      </td>
      <td style={{ width: "150px", height: "100px" }}>
        <Modal
          opened={opened}
          size="xl"
          title={item?.courseName}
          onClose={() => setOpened(false)}
        >
          <Image src={item?.certificateUrl}></Image>
        </Modal>
        <div style={{ position: "relative", width: "150px", height: "100px" }}>
          <Anchor onClick={() => setOpened((v) => !v)}>
            <Image
              width={150}
              height={100}
              fit="contain"
              // sx={{":hover"}}
              src={item?.certificateUrl}
            />
          </Anchor>
          <Flex
            justify={"center"}
            align={"center"}
            style={{
              position: "absolute",
              left: 0,
              bottom: 0,
              right: 0,
              top: 0,
              width: "100%",
              height: "100%",
            }}
          >
            <CopyButton value={item?.certificateUrl} timeout={2000}>
              {({ copied, copy }) => (
                <Tooltip
                  label={copied ? t('copied') : t('copy')}
                  withArrow
                  position="right"
                >
                  <ActionIcon color={copied ? "teal" : "gray"} onClick={copy}>
                    {copied ? <IconCheck size={18} /> : <IconEye size={18} />}
                  </ActionIcon>
                </Tooltip>
              )}
            </CopyButton>
            <ActionIcon onClick={() => handleDownload()}>
              <IconDownload />
            </ActionIcon>
          </Flex>
        </div>
      </td>
    </tr>
  );
};

const InternalCertificate = () => {
  const { id } = useParams();
  const { data, isSuccess } = useProfileAuth(id as string);
  const { t } = useTranslation();
  return (
    <>
      <Title mt={"xl"}>{t("certificate")}</Title>
      {data && data?.certificates.length > 0 ? (
        <ScrollArea>
          <Paper mt={10}>
            <Table
              sx={{ minWidth: 800 }}
              verticalSpacing="sm"
              striped
              highlightOnHover
              withBorder
            >
              <thead>
                <tr>
                  <th>{t("training_name")}</th>
                  <th>{t("completion")}</th>
                  <th>{t("is_issued")}</th>
                  <th>{t("certificate_url")}</th>
                </tr>
              </thead>
              <tbody>
                {data?.certificates &&
                  data?.certificates.map((x: any) => (
                    <RowsCompleted key={x.userId} item={x} t={t} />
                  ))}
              </tbody>
            </Table>
          </Paper>
        </ScrollArea>
      ) : (
        <Box mt={10}>{t("no_certificate")}</Box>
      )}
    </>
  );
};

export default InternalCertificate;
