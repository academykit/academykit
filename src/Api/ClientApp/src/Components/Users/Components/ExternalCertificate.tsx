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
  useMantineTheme,
} from "@mantine/core";
import { IconCheck, IconCopy, IconDownload } from "@tabler/icons";
import {
  GetExternalCertificate,
  useGetUserCertificate,
} from "@utils/services/certificateService";
import moment from "moment";
import { useState } from "react";
import { useParams } from "react-router-dom";

const RowsExternal = ({ item }: { item: GetExternalCertificate }) => {
  const theme = useMantineTheme();
  const [opened, setOpened] = useState(false);
  const handleDownload = () => {
    window.open(item?.imageUrl);
  };
  return (
    <tr key={item?.user?.id}>
      <td>{item?.name}</td>

      <td>{item?.status === 2 ? <Badge>Yes</Badge> : <Badge>No</Badge>}</td>
      <td>{moment(item?.startDate).format(theme.dateFormat)}</td>
      <td>{moment(item?.endDate).format(theme.dateFormat)}</td>
      <td>{item?.institute}</td>
      <td>{item.location}</td>
      <td style={{ maxWidth: "0px" }}>
        <Modal
          opened={opened}
          size="xl"
          title={item?.name}
          onClose={() => setOpened(false)}
        >
          <Image src={item?.imageUrl}></Image>
        </Modal>
        <Flex align={"center"}>
          <Anchor onClick={() => setOpened((v) => !v)}>
            {item?.imageUrl && (
              <Image
                width={150}
                height={100}
                fit="contain"
                // sx={{":hover"}}
                src={item?.imageUrl}
              />
            )}
          </Anchor>
          <CopyButton value={item?.imageUrl} timeout={2000}>
            {({ copied, copy }) => (
              <Tooltip
                label={copied ? "Copied" : "Copy"}
                withArrow
                position="right"
              >
                <ActionIcon color={copied ? "teal" : "gray"} onClick={copy}>
                  {copied ? <IconCheck size={16} /> : <IconCopy size={16} />}
                </ActionIcon>
              </Tooltip>
            )}
          </CopyButton>
          <ActionIcon onClick={() => handleDownload()}>
            <IconDownload />
          </ActionIcon>
        </Flex>
      </td>
    </tr>
  );
};
const ExternalCertificate = () => {
  const { id } = useParams();
  const externalCertificate = useGetUserCertificate(id as string);

  return (
    <>
      {externalCertificate.data && externalCertificate.data.length > 0 && (
        <>
          <Title mt={"xl"}>External Certificate</Title>
          <ScrollArea>
            <Paper mt={10}>
              <Table
                sx={{ minWidth: 800 }}
                verticalSpacing="sm"
                striped
                highlightOnHover
              >
                <thead>
                  <tr>
                    <th>Trainings Name</th>
                    <th>Verified</th>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Issued by</th>
                    <th>Issuer location</th>
                    <th>Certificate URL</th>
                  </tr>
                </thead>
                <tbody>
                  {externalCertificate?.data.map((x: any) => (
                    <RowsExternal key={x.userId} item={x} />
                  ))}
                </tbody>
              </Table>
            </Paper>
          </ScrollArea>
        </>
      )}
    </>
  );
};

export default ExternalCertificate;
