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
} from "@mantine/core";
import { IconCheck, IconCopy, IconDownload } from "@tabler/icons";
import { useProfileAuth } from "@utils/services/authService";
import { ICertificateList } from "@utils/services/manageCourseService";
import { useState } from "react";
import { useParams } from "react-router-dom";
const RowsCompleted = ({ item }: { item: ICertificateList }) => {
  const [opened, setOpened] = useState(false);
  const handleDownload = () => {
    window.open(item?.certificateUrl);
  };
  return (
    <tr key={item?.user?.id}>
      <td>{item?.courseName}</td>

      <td>{item?.percentage}%</td>
      <td>
        {item?.hasCertificateIssued ? <Badge>Yes</Badge> : <Badge>No</Badge>}
      </td>
      <td style={{ maxWidth: "0px" }}>
        <Modal
          opened={opened}
          size="xl"
          title={item?.courseName}
          onClose={() => setOpened(false)}
        >
          <Image src={item?.certificateUrl}></Image>
        </Modal>
        <Flex align={"center"}>
          <Anchor onClick={() => setOpened((v) => !v)}>
            <Image
              width={150}
              height={100}
              fit="contain"
              // sx={{":hover"}}
              src={item?.certificateUrl}
            />
          </Anchor>
          <CopyButton value={item?.certificateUrl} timeout={2000}>
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

const InternalCertificate = () => {
  const { id } = useParams();
  const { data, isSuccess } = useProfileAuth(id as string);
  return (
    <>
      <Title mt={"xl"}>Certificate</Title>
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
                <th>Completion</th>
                <th>isIssued</th>
                <th>Certificate URL</th>
              </tr>
            </thead>
            <tbody>
              {data?.certificates &&
                data?.certificates.map((x: any) => (
                  <RowsCompleted key={x.userId} item={x} />
                ))}
            </tbody>
          </Table>
        </Paper>
      </ScrollArea>
    </>
  );
};

export default InternalCertificate;
