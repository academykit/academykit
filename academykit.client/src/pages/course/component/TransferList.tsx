import TextViewer from "@components/Ui/RichTextViewer";
import {
  ActionIcon,
  Checkbox,
  Combobox,
  Group,
  Text,
  TextInput,
  useCombobox,
} from "@mantine/core";
import {
  IconChevronRight,
  IconChevronsRight,
  IconPlus,
} from "@tabler/icons-react";
import { Dispatch, SetStateAction, useState } from "react";
import { useTranslation } from "react-i18next";
import { IQuestionListData } from "../question";
import classes from "./TransferList.module.css";

interface RenderListProps {
  options: IQuestionListData[];
  onTransfer(options: IQuestionListData[]): void;
  onTransferAll(): void;
  type: "forward" | "backward";
  openModal?: () => void;
}

function RenderList({
  options,
  onTransfer,
  type,
  onTransferAll,
  openModal,
}: RenderListProps) {
  const combobox = useCombobox();
  const [value, setValue] = useState<IQuestionListData[]>([]);
  const [search, setSearch] = useState("");
  const { t } = useTranslation();

  // find the object which matches the val i.e., value
  // if the object is already in the value array, remove it
  // else add it to the value array
  const handleValueSelect = (val: string) => {
    setValue((current) =>
      current.some((item) => item.value === val)
        ? current.filter((item) => item.value !== val)
        : [...current, options.find((item) => item.value === val)!]
    );
  };

  const items = options
    .filter((item) =>
      item.label?.toLowerCase().includes(search.toLowerCase().trim())
    )
    .map((item) => (
      <Combobox.Option
        value={item.value}
        key={item.value}
        active={value.includes(item)}
        onMouseOver={() => combobox.resetSelectedOption()}
      >
        <Group wrap="nowrap">
          <Checkbox
            checked={value.includes(item)}
            onChange={() => {}}
            aria-hidden
            tabIndex={-1}
            style={{ pointerEvents: "none" }}
          />
          <div style={{ flex: 1 }}>
            <Text size="sm" fw={500}>
              {item.label}
            </Text>
            {item && item?.description !== null && (
              <Text lineClamp={3} style={{ overflow: "hidden" }}>
                <TextViewer
                  content={item?.description}
                  styles={{
                    wordBreak: "break-all",
                  }}
                />
              </Text>
            )}
          </div>
        </Group>
      </Combobox.Option>
    ));

  return (
    <div className={classes.renderList} data-type={type}>
      <Combobox store={combobox} onOptionSubmit={handleValueSelect}>
        <Combobox.EventsTarget>
          <Group wrap="nowrap" gap={0} className={classes.controls}>
            {type == "backward" && (
              <ActionIcon
                variant="default"
                size={36}
                className={classes.addControl}
                onClick={() => {
                  // open modal
                  openModal && openModal();
                }}
              >
                <IconPlus className={classes.icon} />
              </ActionIcon>
            )}
            <TextInput
              placeholder={t("search_for_questions") as string}
              classNames={{ input: classes.input }}
              value={search}
              onChange={(event) => {
                setSearch(event.currentTarget.value);
                combobox.updateSelectedOptionIndex();
              }}
              style={{ width: "100%" }}
            />
            <ActionIcon
              style={{ borderRadius: 0 }}
              variant="default"
              size={36}
              className={classes.control}
              onClick={() => {
                onTransfer(value);
                setValue([]);
              }}
            >
              <IconChevronRight className={classes.icon} />
            </ActionIcon>
            <ActionIcon
              radius={0}
              variant="default"
              size={36}
              className={classes.control}
              onClick={() => {
                onTransferAll();
                setValue([]);
              }}
            >
              <IconChevronsRight className={classes.icon} />
            </ActionIcon>
          </Group>
        </Combobox.EventsTarget>

        <div className={classes.list}>
          <Combobox.Options>
            {items.length > 0 ? (
              items
            ) : (
              <Combobox.Empty>{t("no_question_found")}</Combobox.Empty>
            )}
          </Combobox.Options>
        </div>
      </Combobox>
    </div>
  );
}

interface IProps {
  data: any;
  setData: Dispatch<SetStateAction<any[]>>;
  openModal: () => void;
}
export default function TransferList({ data, setData, openModal }: IProps) {
  // transfer selected items to the opposite container
  const handleTransfer = (
    transferFrom: number,
    options: IQuestionListData[]
  ) => {
    setData((current) => {
      const transferTo = transferFrom === 0 ? 1 : 0;
      const transferFromData = current[transferFrom].filter(
        (item: any) => !options.includes(item)
      );
      const transferToData = [...current[transferTo], ...options];
      const result = [];
      result[transferFrom] = transferFromData;
      result[transferTo] = transferToData;

      return result as [any[], any[]];
    });
  };

  // transfer all items to the opposite container
  const handleTransferAll = (transferFrom: number) => {
    setData((current) => {
      const transferTo = transferFrom === 0 ? 1 : 0;
      const transferFromData = current[transferFrom];
      const transferToData = [...current[transferTo], ...transferFromData];
      const result = [];
      result[transferFrom] = [];
      result[transferTo] = transferToData;

      return result as [any[], any[]];
    });
  };

  return (
    <div className={classes.root}>
      <RenderList
        type="forward"
        options={data[0]}
        onTransfer={(options) => handleTransfer(0, options)}
        onTransferAll={() => handleTransferAll(0)}
      />
      <RenderList
        type="backward"
        options={data[1]}
        onTransfer={(options) => handleTransfer(1, options)}
        onTransferAll={() => handleTransferAll(1)}
        openModal={openModal}
      />
    </div>
  );
}
