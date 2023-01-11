import DeleteModal from "@components/Ui/DeleteModal";
import {
  ActionIcon,
  Box,
  Button,
  Container,
  createStyles,
  Group,
  Modal,
  Paper,
  Switch,
  Table,
  Text,
  TextInput,
  Title,
  Transition,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { randomId, useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconPencil, IconTrash } from "@tabler/icons";
import {
  useUpdateLevelSetting,
  useLevelSetting,
  usePostLevelSetting,
  useDeleteLevelSetting,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUser } from "@utils/services/types";
import { useState } from "react";
import * as Yup from "yup";
interface ILevel<T> {
  id: string;
  name: string;
  user: T;
}

const useStyles = createStyles((theme) => ({
  paper: {
    [theme.fn.smallerThan("md")]: {
      width: "100%",
    },
    [theme.fn.smallerThan("lg")]: {
      width: "100%",
    },

    width: "50%",
    marginBottom: "20px",
  },
}));

const schema = Yup.object().shape({
  name: Yup.string().required("Level name is required."),
});

const Level = () => {
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema),
  });
  const { classes } = useStyles();

  const Rows = ({ item }: { item: ILevel<IUser> }) => {
    const [opened, setOpened] = useState(false);
    const [isEdit, setIsEdit] = useState<boolean>(false);
    const updateLevel = useUpdateLevelSetting(item.id);

    const form = useForm({
      initialValues: {
        eName: item.name,
      },
    });
    const deleteLevel = useDeleteLevelSetting();
    const handleDelete = async () => {
      try {
        await deleteLevel.mutateAsync(item.id);
        showNotification({
          message: "Level deleted successfully!",
        });
      } catch (error) {
        const err = errorType(error);
        showNotification({
          message: err,
          color: "red",
        });
      }
      setOpened(false);
    };

    return (
      <tr key={item.id}>
        <Modal opened={isEdit} onClose={() => setIsEdit(false)}>
          <Box mt={10}>
            <form
              onSubmit={form.onSubmit(async (data) => {
                try {
                  const up = await updateLevel.mutateAsync({
                    name: data.eName,
                    id: item.id,
                  });
                  showNotification({ message: "Successfully Updated." });
                } catch (error) {
                  const err = errorType(error);
                  showNotification({
                    message: err,
                    color: "red",
                  });
                }
              })}
            >
              <Container
                size={450}
                sx={{
                  marginLeft: "0px",
                }}
              >
                <TextInput
                  withAsterisk
                  label="Level Name"
                  placeholder="Enter Level Name"
                  name="eName"
                  {...form.getInputProps("eName")}
                />
              </Container>

              <Group mt={20} ml={10}>
                <Button type="submit" variant="outline">
                  Save
                </Button>
              </Group>
            </form>
          </Box>
        </Modal>
        <DeleteModal
          title={`Do you want to delete "${item?.name}" level?`}
          open={opened}
          onClose={setOpened}
          onConfirm={handleDelete}
        />

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item.name}
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing={0} position="center">
            <ActionIcon>
              <IconPencil
                size={16}
                stroke={1.5}
                onClick={() => setIsEdit(true)}
              />
            </ActionIcon>
            <ActionIcon color="red">
              <IconTrash
                size={16}
                stroke={1.5}
                onClick={() => {
                  setOpened(true);
                }}
              />
            </ActionIcon>
          </Group>
        </td>
      </tr>
    );
  };

  const getLevel = useLevelSetting();
  const postLevel = usePostLevelSetting();

  const [showAddForm, toggleAddForm] = useToggle();

  return (
    <>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>Levels</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>Add Level</Button>
        )}
      </Group>

      <Transition
        mounted={showAddForm}
        transition={"slide-down"}
        duration={200}
        timingFunction="ease"
      >
        {(style) => (
          <Paper
            shadow={"sm"}
            radius="md"
            p="xl"
            withBorder
            className={classes.paper}
          >
            <Box mt={10}>
              <form
                onSubmit={form.onSubmit(async (values) => {
                  try {
                    await postLevel.mutateAsync(values);
                    showNotification({
                      message: "Successfully added level!",
                    });
                    form.reset();
                    toggleAddForm();
                  } catch (error) {
                    const err = errorType(error);

                    showNotification({
                      title: "Error!",
                      message: err,
                      color: "red",
                    });
                  }
                })}
              >
                <TextInput
                  label="Level Name"
                  name="levelName"
                  withAsterisk
                  placeholder="Enter Level Name."
                  {...form.getInputProps("name")}
                />

                <Group mt={10}>
                  <Button type="submit">Submit</Button>
                  {showAddForm && (
                    <Button onClick={() => toggleAddForm()} variant="outline">
                      Cancel
                    </Button>
                  )}
                </Group>
              </form>
            </Box>
          </Paper>
        )}
      </Transition>

      <Paper>
        <Table striped highlightOnHover withBorder sx={{ marginTop: "10px" }}>
          <thead>
            <tr>
              <th>Name</th>

              <th>
                <Text align="center">Actions</Text>
              </th>
            </tr>
          </thead>
          <tbody>
            {getLevel.data?.map((item: any) => (
              <Rows item={item} key={item.id} />
            ))}
          </tbody>
        </Table>
      </Paper>
    </>
  );
};

export default Level;
